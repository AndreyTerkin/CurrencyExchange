using CurrencyWorker.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CurrencyWorker;

public class Worker(
    IServiceScopeFactory scopeFactory,
    CbrXmlParser cbrParser,
    IConfiguration configuration,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("CurrencyWorker started");

        TimeSpan interval = TimeSpan.FromMinutes(
            configuration.GetValue<int>("CurrencyWorker:SyncIntervalMinutes", 60));

        await SyncRatesAsync(stoppingToken);

        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SyncRatesAsync(stoppingToken);
        }

        logger.LogInformation("CurrencyWorker stopped");
    }

    private async Task SyncRatesAsync(CancellationToken ct)
    {
        logger.LogInformation("Currency sync started");
        try
        {
            IReadOnlyList<CbrCurrencyEntry> entries = await cbrParser.FetchRatesAsync(ct);

            using IServiceScope scope = scopeFactory.CreateScope();
            WorkerDbContext db = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();

            Dictionary<string, CurrencyEntity> existing =
                await db.Currencies.ToDictionaryAsync(c => c.Code, ct);

            foreach (CbrCurrencyEntry entry in entries)
            {
                if (existing.TryGetValue(entry.Code, out CurrencyEntity? entity))
                {
                    entity.Name = entry.Name;
                    entity.Rate = entry.Rate;
                }
                else
                {
                    db.Currencies.Add(CurrencyEntity.Create(entry.Code, entry.Name, entry.Rate));
                }
            }

            await db.SaveChangesAsync(ct);
            logger.LogInformation("Currency sync complete: {Count} currencies processed", entries.Count);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Currency sync failed");
        }
    }
}
