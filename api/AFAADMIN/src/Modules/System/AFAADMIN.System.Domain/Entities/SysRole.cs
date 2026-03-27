using AFAADMIN.Database.Entities;
using SqlSugar;

namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 系统角色
/// </summary>
[SugarTable("sys_role", "系统角色表")]
public class SysRole : BaseEntity
{
    [SugarColumn(Length = 64, IsNullable = false, ColumnDescription = "角色名称")]
    public string RoleName { get; set; } = string.Empty;

    [SugarColumn(Length = 64, IsNullable = false, ColumnDescription = "角色编码")]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 排序（越小越靠前）
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态（1=正常 0=停用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}
