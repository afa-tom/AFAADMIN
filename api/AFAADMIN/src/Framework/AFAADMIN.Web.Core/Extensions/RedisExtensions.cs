using AFAADMIN.Common.Cache;
using AFAADMIN.Common.Config;
using AFAADMIN.Web.Core.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace AFAADMIN.Web.Core.Extensions;

public static class RedisExtensions
{
    /// <summary>
    /// 注册 Redis 连接 + 缓存服务
    /// </summary>
    public static IServiceCollection AddAfaRedis(this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = new RedisConfig();
        configuration.GetSection("Redis").Bind(config);
        services.AddSingleton(config);

        // 注册 Redis 连接
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RedisCacheService>>();
            try
            {
                var connection = ConnectionMultiplexer.Connect(config.ConnectionString);
                logger.LogInformation("Redis 连接成功: {Endpoint}", config.ConnectionString.Split(',')[0]);
                return connection;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis 连接失败，将使用内存缓存降级");
                throw;
            }
        });

        // 注册缓存服务
        services.AddSingleton<ICacheService>(sp =>
        {
            var redis = sp.GetRequiredService<IConnectionMultiplexer>();
            var logger = sp.GetRequiredService<ILogger<RedisCacheService>>();
            return new RedisCacheService(redis, config.InstanceName, logger);
        });

        return services;
    }
}
