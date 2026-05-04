using StackExchange.Redis;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Services;

public class RedisTokenBlacklistService(IConnectionMultiplexer redis) : ITokenBlacklistService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public Task BlacklistAsync(string jti, TimeSpan ttl, CancellationToken ct = default) =>
        _db.StringSetAsync($"blacklist:{jti}", "1", ttl);

    public Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default) =>
        _db.KeyExistsAsync($"blacklist:{jti}");
}
