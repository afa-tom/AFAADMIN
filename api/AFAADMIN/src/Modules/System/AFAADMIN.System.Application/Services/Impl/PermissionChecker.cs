using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.System.Domain.Entities;
using AFAADMIN.Web.Core.Authentication;
using SqlSugar;
using ICacheService = AFAADMIN.Common.Cache.ICacheService;
using CacheKeys = AFAADMIN.Common.Cache.CacheKeys;

namespace AFAADMIN.System.Application.Services.Impl;

public class PermissionChecker : IPermissionChecker, IScopedDependency
{
    private readonly ISqlSugarClient _db;
    private readonly ICacheService _cache;

    public PermissionChecker(ISqlSugarClient db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(long userId, string permissionCode)
    {
        var permissions = await GetPermissionsAsync(userId);
        return permissions.Contains("*:*:*") || permissions.Contains(permissionCode);
    }

    public async Task<List<string>> GetPermissionsAsync(long userId)
    {
        // 先查缓存
        var cached = await _cache.GetAsync<List<string>>(CacheKeys.UserPermissions(userId));
        if (cached != null) return cached;

        // 检查是否超管
        var roles = await _db.Queryable<SysUserRole>()
            .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
            .Where((ur, r) => ur.UserId == userId && r.Status == 1)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        List<string> permissions;
        if (roles.Contains("admin"))
        {
            permissions = ["*:*:*"];
        }
        else
        {
            permissions = await _db.Queryable<SysUserRole>()
                .LeftJoin<SysRoleMenu>((ur, rm) => ur.RoleId == rm.RoleId)
                .LeftJoin<SysMenu>((ur, rm, m) => rm.MenuId == m.Id)
                .Where((ur, rm, m) => ur.UserId == userId
                    && m.Status == 1 && m.Permission != null && m.Permission != "")
                .Select((ur, rm, m) => m.Permission!)
                .Distinct()
                .ToListAsync();
        }

        // 写入缓存
        await _cache.SetAsync(CacheKeys.UserPermissions(userId), permissions,
            TimeSpan.FromMinutes(30));

        return permissions;
    }
}
