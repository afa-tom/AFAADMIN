using System.Collections.Concurrent;
using System.Reflection;
using AFAADMIN.Common.Attributes;
using AFAADMIN.Common.Crypto;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.Database.Encryption;

/// <summary>
/// 敏感字段加解密服务 — 处理标记了 [SensitiveField] 的实体属性
/// </summary>
public class SensitiveFieldEncryptor
{
    private readonly string _sm4Key;
    private readonly ILogger<SensitiveFieldEncryptor> _logger;

    // 缓存每个类型的敏感字段列表，避免重复反射
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _cache = new();

    public SensitiveFieldEncryptor(string sm4Key, ILogger<SensitiveFieldEncryptor> logger)
    {
        _sm4Key = sm4Key;
        _logger = logger;
    }

    /// <summary>
    /// 获取类型的所有敏感字段
    /// </summary>
    private static PropertyInfo[] GetSensitiveProperties(Type type)
    {
        return _cache.GetOrAdd(type, t =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<SensitiveFieldAttribute>() != null
                         && p.PropertyType == typeof(string)
                         && p.CanRead && p.CanWrite)
                .ToArray());
    }

    /// <summary>
    /// 加密实体中的敏感字段（写入前调用）
    /// </summary>
    public void Encrypt<T>(T entity) where T : class
    {
        if (entity == null || string.IsNullOrEmpty(_sm4Key)) return;

        var props = GetSensitiveProperties(typeof(T));
        foreach (var prop in props)
        {
            var value = prop.GetValue(entity) as string;
            if (string.IsNullOrEmpty(value)) continue;

            try
            {
                prop.SetValue(entity, SM4Helper.EncryptECB(value, _sm4Key));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "敏感字段 {Property} 加密失败", prop.Name);
            }
        }
    }

    /// <summary>
    /// 解密实体中的敏感字段（读取后调用）
    /// </summary>
    public void Decrypt<T>(T entity) where T : class
    {
        if (entity == null || string.IsNullOrEmpty(_sm4Key)) return;

        var props = GetSensitiveProperties(typeof(T));
        foreach (var prop in props)
        {
            var value = prop.GetValue(entity) as string;
            if (string.IsNullOrEmpty(value)) continue;

            try
            {
                prop.SetValue(entity, SM4Helper.DecryptECB(value, _sm4Key));
            }
            catch (Exception ex)
            {
                // 解密失败可能是未加密的老数据，保持原值
                _logger.LogDebug(ex, "敏感字段 {Property} 解密失败（可能为未加密数据）", prop.Name);
            }
        }
    }

    /// <summary>
    /// 批量解密
    /// </summary>
    public void DecryptList<T>(IEnumerable<T> entities) where T : class
    {
        foreach (var entity in entities)
        {
            Decrypt(entity);
        }
    }
}
