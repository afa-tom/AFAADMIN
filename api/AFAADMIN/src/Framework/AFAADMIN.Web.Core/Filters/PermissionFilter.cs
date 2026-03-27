using System.Security.Claims;
using AFAADMIN.Common.Models;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.Web.Core.Filters;

/// <summary>
/// 权限校验过滤器 — 拦截标记了 [Permission] 的接口，检查当前用户权限
/// </summary>
public class PermissionFilter : IAsyncAuthorizationFilter
{
    private readonly ILogger<PermissionFilter> _logger;

    public PermissionFilter(ILogger<PermissionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 获取 Action 上的 [Permission] 特性
        var permAttrs = context.ActionDescriptor.EndpointMetadata
            .OfType<PermissionAttribute>()
            .ToList();

        if (permAttrs.Count == 0) return;

        // 未登录
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new ObjectResult(ApiResult.Fail("未登录", 401)) { StatusCode = 200 };
            return;
        }

        // 获取权限检查器
        var checker = context.HttpContext.RequestServices.GetService<IPermissionChecker>();
        if (checker == null)
        {
            _logger.LogWarning("IPermissionChecker 未注册，跳过权限校验");
            return;
        }

        // 超级管理员跳过（角色中包含 admin）
        var roles = context.HttpContext.User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value).ToList();
        if (roles.Contains("admin")) return;

        // 逐个检查权限
        var userPermissions = await checker.GetPermissionsAsync(userId);
        foreach (var attr in permAttrs)
        {
            if (!userPermissions.Contains(attr.Code))
            {
                _logger.LogWarning("用户 {UserId} 缺少权限: {Permission}", userId, attr.Code);
                context.Result = new ObjectResult(
                    ApiResult.Fail($"无权限: {attr.Code}", 403)) { StatusCode = 200 };
                return;
            }
        }
    }
}
