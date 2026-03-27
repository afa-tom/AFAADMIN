using SqlSugar;

namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 角色-菜单关联（多对多）
/// </summary>
[SugarTable("sys_role_menu", "角色菜单关联表")]
public class SysRoleMenu
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "角色ID")]
    public long RoleId { get; set; }

    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "菜单ID")]
    public long MenuId { get; set; }
}
