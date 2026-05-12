using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using FinanceService.Domain.Entities;
using FinanceService.Infrastructure.Persistence;

namespace FinanceService.IntegrationTests.Infrastructure;

[Collection(IntegrationTestCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    private readonly FinanceServiceWebApplicationFactory _factory;

    protected IntegrationTestBase(FinanceServiceWebApplicationFactory factory)
    {
        _factory = factory;
        Client = factory.CreateClient();
    }

    public Task InitializeAsync() => ClearDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    protected string GenerateToken(int userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(FinanceServiceWebApplicationFactory.JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "currency-exchange",
            audience: "currency-exchange",
            claims: [
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ],
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected async Task<int[]> SeedCurrenciesAsync(params (string Code, string Name, decimal Rate)[] currencies)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        var entities = currencies
            .Select(c => FinanceService.Domain.Entities.Currency.Create(0, c.Code, c.Name, c.Rate))
            .ToArray();
        db.Currencies.AddRange(entities);
        await db.SaveChangesAsync();
        return entities.Select(e => e.Id).ToArray();
    }

    protected async Task SeedFavoritesAsync(int userId, params int[] currencyIds)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        db.UserFavoriteCurrencies.AddRange(currencyIds.Select(cid =>
            new UserFavoriteCurrency { UserId = userId, CurrencyId = cid }));
        await db.SaveChangesAsync();
    }

    private async Task ClearDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        await db.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE user_favorite_currencies, currency RESTART IDENTITY CASCADE");
    }
}
