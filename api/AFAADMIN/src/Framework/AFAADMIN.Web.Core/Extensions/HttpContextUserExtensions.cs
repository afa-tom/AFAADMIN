using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AFAADMIN.Web.Core.Extensions;

/// <summary>
/// HttpContext 用户信息扩展
/// </summary>
public static class HttpContextUserExtensions
{
    /// <summary>
    /// 获取当前登录用户 ID
    /// </summary>
    public static long GetUserId(this HttpContext context)
    {
        var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && long.TryParse(claim.Value, out var id) ? id : 0;
    }

    /// <summary>
    /// 获取当前登录用户名
    /// </summary>
    public static string GetUserName(this HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    /// <summary>
    /// 获取当前用户角色列表
    /// </summary>
    public static List<string> GetUserRoles(this HttpContext context)
    {
        return context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public static bool IsSuperAdmin(this HttpContext context)
    {
        return context.GetUserRoles().Contains("admin");
    }
}
