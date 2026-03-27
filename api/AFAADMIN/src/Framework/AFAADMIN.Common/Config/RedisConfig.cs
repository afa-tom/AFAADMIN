namespace AFAADMIN.Common.Config;

/// <summary>
/// Redis 配置（映射 configs/redis.json）
/// </summary>
public class RedisConfig
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "AFAADMIN:";
    public bool EnableDistributedCache { get; set; } = true;
    public int DefaultExpirationMinutes { get; set; } = 30;
}
