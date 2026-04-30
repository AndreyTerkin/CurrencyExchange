using DbMigrator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .UseContentRoot(AppContext.BaseDirectory)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration["ConnectionStrings:DefaultConnection"]!;
        services.AddDbContext<MigratorDbContext>(options =>
            options.UseNpgsql(connectionString));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<MigratorDbContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<MigratorDbContext>>();

logger.LogInformation("Applying database migrations...");
await db.Database.MigrateAsync();
logger.LogInformation("Migrations applied successfully.");
