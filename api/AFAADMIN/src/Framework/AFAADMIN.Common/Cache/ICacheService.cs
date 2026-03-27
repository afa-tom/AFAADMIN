namespace AFAADMIN.Common.Cache;

/// <summary>
/// 缓存服务接口 — 定义在 Common 层，Redis 实现在 Web.Core 层
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 获取缓存
    /// </summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// 获取字符串缓存
    /// </summary>
    Task<string?> GetStringAsync(string key);

    /// <summary>
    /// 设置缓存
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    /// <summary>
    /// 设置字符串缓存
    /// </summary>
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null);

    /// <summary>
    /// 删除缓存
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// 按前缀批量删除
    /// </summary>
    Task RemoveByPrefixAsync(string prefix);

    /// <summary>
    /// 判断是否存在
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 获取或设置缓存（不存在则通过工厂方法创建并缓存）
    /// </summary>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);
}
