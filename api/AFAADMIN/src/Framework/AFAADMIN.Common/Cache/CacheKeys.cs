namespace AFAADMIN.Common.Cache;

/// <summary>
/// 缓存 Key 常量定义
/// </summary>
public static class CacheKeys
{
    private const string Prefix = "afa:";

    // ===== Token 相关 =====
    /// <summary>
    /// Token 黑名单: afa:token:blacklist:{jti}
    /// </summary>
    public static string TokenBlacklist(string jti) => $"{Prefix}token:blacklist:{jti}";

    /// <summary>
    /// RefreshToken: afa:token:refresh:{token}
    /// </summary>
    public static string RefreshToken(string token) => $"{Prefix}token:refresh:{token}";

    // ===== 权限相关 =====
    /// <summary>
    /// 用户权限列表: afa:perm:user:{userId}
    /// </summary>
    public static string UserPermissions(long userId) => $"{Prefix}perm:user:{userId}";

    /// <summary>
    /// 用户角色列表: afa:role:user:{userId}
    /// </summary>
    public static string UserRoles(long userId) => $"{Prefix}role:user:{userId}";

    // ===== 字典相关 =====
    /// <summary>
    /// 字典数据: afa:dict:{dictCode}
    /// </summary>
    public static string DictData(string dictCode) => $"{Prefix}dict:{dictCode}";

    /// <summary>
    /// 字典类型列表: afa:dict:types
    /// </summary>
    public const string DictTypes = $"{Prefix}dict:types";

    // ===== 前缀（用于批量清除） =====
    public const string PermPrefix = $"{Prefix}perm:";
    public const string RolePrefix = $"{Prefix}role:";
    public const string DictPrefix = $"{Prefix}dict:";
    public const string TokenPrefix = $"{Prefix}token:";
}
