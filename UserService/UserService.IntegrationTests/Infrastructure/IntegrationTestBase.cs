using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.Infrastructure.Persistence;

namespace UserService.IntegrationTests.Infrastructure;

[Collection(IntegrationTestCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    private readonly UserServiceWebApplicationFactory _factory;

    protected IntegrationTestBase(UserServiceWebApplicationFactory factory)
    {
        _factory = factory;
        Client = factory.CreateClient();
    }

    public Task InitializeAsync() => ClearDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task ClearDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE users RESTART IDENTITY CASCADE");
    }
}
