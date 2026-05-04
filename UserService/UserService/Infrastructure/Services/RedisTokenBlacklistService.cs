using StackExchange.Redis;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Services;

public class RedisTokenBlacklistService(
    IConnectionMultiplexer redis,
    ILogger<RedisTokenBlacklistService> logger) : ITokenBlacklistService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task BlacklistAsync(string jti, TimeSpan ttl, CancellationToken ct = default)
    {
        try
        {
            await _db.StringSetAsync($"blacklist:{jti}", "1", ttl);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable — token {Jti} could not be blacklisted", jti);
        }
    }

    public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default)
    {
        try
        {
            return await _db.KeyExistsAsync($"blacklist:{jti}");
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable — blacklist check skipped for token {Jti}", jti);
            return false;
        }
    }
}
