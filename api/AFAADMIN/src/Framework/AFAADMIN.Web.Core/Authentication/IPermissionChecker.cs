namespace AFAADMIN.Web.Core.Authentication;

/// <summary>
/// 权限检查接口 — Web.Core 层定义，业务模块实现
/// </summary>
public interface IPermissionChecker
{
    /// <summary>
    /// 检查指定用户是否拥有某权限标识
    /// </summary>
    Task<bool> HasPermissionAsync(long userId, string permissionCode);

    /// <summary>
    /// 获取用户所有权限标识
    /// </summary>
    Task<List<string>> GetPermissionsAsync(long userId);
}
