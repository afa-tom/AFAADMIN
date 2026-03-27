using SqlSugar;

namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 用户-角色关联（多对多）
/// </summary>
[SugarTable("sys_user_role", "用户角色关联表")]
public class SysUserRole
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "用户ID")]
    public long UserId { get; set; }

    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "角色ID")]
    public long RoleId { get; set; }
}
