using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SqlSugar;

namespace AFAADMIN.AI.Functions;

/// <summary>
/// 系统查询 Native Functions — SK 引擎可调用的业务函数
/// 通过 Function Calling 让 AI 具备操作系统数据的能力
/// </summary>
public class SystemQueryFunctions
{
    private readonly IServiceProvider _serviceProvider;

    public SystemQueryFunctions(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [KernelFunction("get_user_count")]
    [Description("获取系统用户总数，可按状态筛选。status: 1=正常, 0=停用, 不传则查全部")]
    public async Task<string> GetUserCountAsync(
        [Description("用户状态: 1=正常, 0=停用")] int? status = null)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var query = db.Queryable<dynamic>().AS("sys_user")
            .Where("IsDeleted = 0");
        if (status.HasValue)
            query = query.Where($"Status = {status.Value}");

        var count = await query.CountAsync();
        var statusText = status switch { 1 => "正常", 0 => "停用", _ => "全部" };
        return $"系统中共有 {count} 个{statusText}用户。";
    }

    [KernelFunction("get_role_list")]
    [Description("获取系统所有角色列表，返回角色名称和编码")]
    public async Task<string> GetRoleListAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var roles = await db.Queryable<dynamic>().AS("sys_role")
            .Where("IsDeleted = 0")
            .Select<dynamic>("RoleName, RoleCode, Status")
            .ToListAsync();

        if (roles.Count == 0) return "系统中暂无角色。";

        var lines = new List<string> { $"系统中共有 {roles.Count} 个角色：" };
        foreach (var r in roles)
        {
            var dict = (IDictionary<string, object>)r;
            var name = dict["RoleName"]?.ToString() ?? "";
            var code = dict["RoleCode"]?.ToString() ?? "";
            var st = Convert.ToInt32(dict["Status"]) == 1 ? "正常" : "停用";
            lines.Add($"- {name}（{code}）[{st}]");
        }
        return string.Join("\n", lines);
    }

    [KernelFunction("get_dept_tree")]
    [Description("获取部门组织架构树形结构")]
    public async Task<string> GetDeptTreeAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var depts = await db.Queryable<dynamic>().AS("sys_dept")
            .Where("IsDeleted = 0")
            .OrderBy("Sort ASC")
            .Select<dynamic>("Id, ParentId, DeptName, Leader, Status")
            .ToListAsync();

        if (depts.Count == 0) return "系统中暂无部门。";

        var lines = new List<string> { $"系统中共有 {depts.Count} 个部门：" };
        foreach (var d in depts)
        {
            var dict = (IDictionary<string, object>)d;
            var name = dict["DeptName"]?.ToString() ?? "";
            var leader = dict["Leader"]?.ToString();
            var leaderText = string.IsNullOrEmpty(leader) ? "" : $"，负责人: {leader}";
            lines.Add($"- {name}{leaderText}");
        }
        return string.Join("\n", lines);
    }

    [KernelFunction("search_users")]
    [Description("按用户名或手机号搜索用户，返回匹配的用户列表")]
    public async Task<string> SearchUsersAsync(
        [Description("搜索关键字（用户名）")] string keyword)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var users = await db.Queryable<dynamic>().AS("sys_user")
            .Where("IsDeleted = 0")
            .Where($"UserName LIKE '%{keyword.Replace("'", "''")}%' OR NickName LIKE '%{keyword.Replace("'", "''")}%'")
            .Select<dynamic>("UserName, NickName, Status, CreateTime")
            .Take(10)
            .ToListAsync();

        if (users.Count == 0) return $"未找到包含 \"{keyword}\" 的用户。";

        var lines = new List<string> { $"找到 {users.Count} 个匹配用户：" };
        foreach (var u in users)
        {
            var dict = (IDictionary<string, object>)u;
            var userName = dict["UserName"]?.ToString() ?? "";
            var nickName = dict["NickName"]?.ToString() ?? userName;
            var st = Convert.ToInt32(dict["Status"]) == 1 ? "正常" : "停用";
            lines.Add($"- {nickName}（{userName}）[{st}]");
        }
        return string.Join("\n", lines);
    }

    [KernelFunction("get_dict_data")]
    [Description("查询字典数据，根据字典编码获取字典选项列表")]
    public async Task<string> GetDictDataAsync(
        [Description("字典编码，如 sys_user_status")] string dictCode)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var items = await db.Queryable<dynamic>().AS("sys_dict_data")
            .LeftJoin<dynamic>((d, t) => true, "sys_dict_type")
            .Where("sys_dict_type.DictCode = @code AND sys_dict_data.IsDeleted = 0",
                new { code = dictCode })
            .Select<dynamic>("sys_dict_data.DictLabel, sys_dict_data.DictValue")
            .ToListAsync();

        if (items.Count == 0) return $"未找到编码为 \"{dictCode}\" 的字典数据。";

        var lines = new List<string> { $"字典 [{dictCode}] 包含 {items.Count} 项：" };
        foreach (var item in items)
        {
            var dict = (IDictionary<string, object>)item;
            lines.Add($"- {dict["DictLabel"]}（值: {dict["DictValue"]}）");
        }
        return string.Join("\n", lines);
    }

    [KernelFunction("get_system_overview")]
    [Description("获取系统总览统计信息，包括用户数、角色数、部门数、菜单数")]
    public async Task<string> GetSystemOverviewAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var userCount = await db.Queryable<dynamic>().AS("sys_user").Where("IsDeleted = 0").CountAsync();
        var roleCount = await db.Queryable<dynamic>().AS("sys_role").Where("IsDeleted = 0").CountAsync();
        var deptCount = await db.Queryable<dynamic>().AS("sys_dept").Where("IsDeleted = 0").CountAsync();
        var menuCount = await db.Queryable<dynamic>().AS("sys_menu").Where("IsDeleted = 0").CountAsync();

        return $"系统总览：\n- 用户总数: {userCount}\n- 角色总数: {roleCount}\n- 部门总数: {deptCount}\n- 菜单总数: {menuCount}";
    }

    [KernelFunction("get_recent_users")]
    [Description("获取最近创建的用户列表")]
    public async Task<string> GetRecentUsersAsync(
        [Description("返回数量，默认5")] int count = 5)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var users = await db.Queryable<dynamic>().AS("sys_user")
            .Where("IsDeleted = 0")
            .OrderBy("CreateTime DESC")
            .Select<dynamic>("UserName, NickName, CreateTime")
            .Take(count)
            .ToListAsync();

        if (users.Count == 0) return "暂无用户数据。";

        var lines = new List<string> { $"最近创建的 {users.Count} 个用户：" };
        foreach (var u in users)
        {
            var dict = (IDictionary<string, object>)u;
            var userName = dict["UserName"]?.ToString() ?? "";
            var nickName = dict["NickName"]?.ToString() ?? userName;
            var time = dict["CreateTime"]?.ToString() ?? "";
            lines.Add($"- {nickName}（{userName}）创建于 {time}");
        }
        return string.Join("\n", lines);
    }
}
