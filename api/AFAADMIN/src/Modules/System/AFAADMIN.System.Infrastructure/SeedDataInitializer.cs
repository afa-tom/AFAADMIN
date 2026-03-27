using AFAADMIN.Common.Crypto;
using AFAADMIN.System.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace AFAADMIN.System.Infrastructure;

/// <summary>
/// 种子数据初始化
/// </summary>
public static class SeedDataInitializer
{
    public static void SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SqlSugarClient>>();

        try
        {
            // 检查是否已有管理员
            if (db.Queryable<SysUser>().Any(u => u.UserName == "admin")) return;

            logger.LogInformation("开始初始化种子数据...");

            // 1. 创建管理员用户
            var salt = SM3Helper.GenerateSalt();
            var adminUser = new SysUser
            {
                Id = 1,
                UserName = "admin",
                NickName = "超级管理员",
                Salt = salt,
                Password = SM3Helper.HashWithSalt("admin123", salt),
                Status = 1
            };
            db.Insertable(adminUser).ExecuteCommand();

            // 2. 创建管理员角色
            var adminRole = new SysRole
            {
                Id = 1,
                RoleName = "超级管理员",
                RoleCode = "admin",
                Sort = 0,
                Status = 1
            };
            db.Insertable(adminRole).ExecuteCommand();

            // 3. 关联用户角色
            db.Insertable(new SysUserRole { UserId = 1, RoleId = 1 }).ExecuteCommand();

            // 4. 创建基础菜单
            var menus = new List<SysMenu>
            {
                new() { Id = 100, ParentId = 0, MenuName = "系统管理", MenuType = 1, Path = "/system", Icon = "setting", Sort = 1 },
                // 用户管理
                new() { Id = 110, ParentId = 100, MenuName = "用户管理", MenuType = 2, Path = "user", Component = "system/user/index", Permission = "sys:user:list", Sort = 1 },
                new() { Id = 111, ParentId = 110, MenuName = "新增用户", MenuType = 3, Permission = "sys:user:add", Sort = 1 },
                new() { Id = 112, ParentId = 110, MenuName = "修改用户", MenuType = 3, Permission = "sys:user:edit", Sort = 2 },
                new() { Id = 113, ParentId = 110, MenuName = "删除用户", MenuType = 3, Permission = "sys:user:delete", Sort = 3 },
                new() { Id = 114, ParentId = 110, MenuName = "重置密码", MenuType = 3, Permission = "sys:user:resetpwd", Sort = 4 },
                // 角色管理
                new() { Id = 120, ParentId = 100, MenuName = "角色管理", MenuType = 2, Path = "role", Component = "system/role/index", Permission = "sys:role:list", Sort = 2 },
                new() { Id = 121, ParentId = 120, MenuName = "新增角色", MenuType = 3, Permission = "sys:role:add", Sort = 1 },
                new() { Id = 122, ParentId = 120, MenuName = "修改角色", MenuType = 3, Permission = "sys:role:edit", Sort = 2 },
                new() { Id = 123, ParentId = 120, MenuName = "删除角色", MenuType = 3, Permission = "sys:role:delete", Sort = 3 },
                // 菜单管理
                new() { Id = 130, ParentId = 100, MenuName = "菜单管理", MenuType = 2, Path = "menu", Component = "system/menu/index", Permission = "sys:menu:list", Sort = 3 },
                new() { Id = 131, ParentId = 130, MenuName = "新增菜单", MenuType = 3, Permission = "sys:menu:add", Sort = 1 },
                new() { Id = 132, ParentId = 130, MenuName = "修改菜单", MenuType = 3, Permission = "sys:menu:edit", Sort = 2 },
                new() { Id = 133, ParentId = 130, MenuName = "删除菜单", MenuType = 3, Permission = "sys:menu:delete", Sort = 3 },
                // 部门管理
                new() { Id = 140, ParentId = 100, MenuName = "部门管理", MenuType = 2, Path = "dept", Component = "system/dept/index", Permission = "sys:dept:list", Sort = 4 },
                new() { Id = 141, ParentId = 140, MenuName = "新增部门", MenuType = 3, Permission = "sys:dept:add", Sort = 1 },
                new() { Id = 142, ParentId = 140, MenuName = "修改部门", MenuType = 3, Permission = "sys:dept:edit", Sort = 2 },
                new() { Id = 143, ParentId = 140, MenuName = "删除部门", MenuType = 3, Permission = "sys:dept:delete", Sort = 3 },
                // 字典管理
                new() { Id = 150, ParentId = 100, MenuName = "字典管理", MenuType = 2, Path = "dict", Component = "system/dict/index", Permission = "sys:dict:list", Sort = 5 },
                new() { Id = 151, ParentId = 150, MenuName = "新增字典", MenuType = 3, Permission = "sys:dict:add", Sort = 1 },
                new() { Id = 152, ParentId = 150, MenuName = "修改字典", MenuType = 3, Permission = "sys:dict:edit", Sort = 2 },
                new() { Id = 153, ParentId = 150, MenuName = "删除字典", MenuType = 3, Permission = "sys:dict:delete", Sort = 3 },
            };
            db.Insertable(menus).ExecuteCommand();

            // 5. 管理员角色分配所有菜单
            var roleMenus = menus.Select(m => new SysRoleMenu { RoleId = 1, MenuId = m.Id }).ToList();
            db.Insertable(roleMenus).ExecuteCommand();

            logger.LogInformation("种子数据初始化完成：admin/admin123");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "种子数据初始化失败");
        }
    }
}
