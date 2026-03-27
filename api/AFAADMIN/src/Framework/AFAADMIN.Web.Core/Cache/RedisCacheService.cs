using System.Text.Json;
using AFAADMIN.Common.Cache;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace AFAADMIN.Web.Core.Cache;

/// <summary>
/// Redis 缓存服务实现
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly string _instanceName;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RedisCacheService(IConnectionMultiplexer redis, string instanceName,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _instanceName = instanceName;
        _logger = logger;
    }

    private string BuildKey(string key) => $"{_instanceName}{key}";

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(BuildKey(key));
        if (!value.HasValue) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "缓存反序列化失败: {Key}", key);
            return default;
        }
    }

    public async Task<string?> GetStringAsync(string key)
    {
        var value = await _db.StringGetAsync(BuildKey(key));
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        await _db.StringSetAsync(BuildKey(key), json, expiry);
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        await _db.StringSetAsync(BuildKey(key), value, expiry);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(BuildKey(key));
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var fullPrefix = BuildKey(prefix);
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{fullPrefix}*").ToArray();
        if (keys.Length > 0)
        {
            await _db.KeyDeleteAsync(keys);
            _logger.LogDebug("批量删除缓存 {Prefix}*, 共 {Count} 个", prefix, keys.Length);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(BuildKey(key));
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached != null) return cached;

        var value = await factory();
        if (value != null)
            await SetAsync(key, value, expiry);

        return value;
    }
}
