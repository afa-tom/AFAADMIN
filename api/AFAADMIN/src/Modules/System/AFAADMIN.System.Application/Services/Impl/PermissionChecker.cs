using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.System.Domain.Entities;
using AFAADMIN.Web.Core.Authentication;
using SqlSugar;

namespace AFAADMIN.System.Application.Services.Impl;

/// <summary>
/// 权限检查器实现
/// </summary>
public class PermissionChecker : IPermissionChecker, IScopedDependency
{
    private readonly ISqlSugarClient _db;

    public PermissionChecker(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<bool> HasPermissionAsync(long userId, string permissionCode)
    {
        var permissions = await GetPermissionsAsync(userId);
        return permissions.Contains("*:*:*") || permissions.Contains(permissionCode);
    }

    public async Task<List<string>> GetPermissionsAsync(long userId)
    {
        // 检查是否超管
        var roles = await _db.Queryable<SysUserRole>()
            .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
            .Where((ur, r) => ur.UserId == userId && r.Status == 1)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        if (roles.Contains("admin"))
            return ["*:*:*"];

        return await _db.Queryable<SysUserRole>()
            .LeftJoin<SysRoleMenu>((ur, rm) => ur.RoleId == rm.RoleId)
            .LeftJoin<SysMenu>((ur, rm, m) => rm.MenuId == m.Id)
            .Where((ur, rm, m) => ur.UserId == userId
                && m.Status == 1 && m.Permission != null && m.Permission != "")
            .Select((ur, rm, m) => m.Permission!)
            .Distinct()
            .ToListAsync();
    }
}
