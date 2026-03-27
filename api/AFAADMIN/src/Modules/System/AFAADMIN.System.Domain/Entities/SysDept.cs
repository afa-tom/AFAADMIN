using AFAADMIN.Database.Entities;
using SqlSugar;

namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 部门/组织机构
/// </summary>
[SugarTable("sys_dept", "部门表")]
public class SysDept : BaseEntity
{
    [SugarColumn(ColumnDescription = "父级ID")]
    public long ParentId { get; set; } = 0;

    [SugarColumn(Length = 64, IsNullable = false, ColumnDescription = "部门名称")]
    public string DeptName { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 负责人
    /// </summary>
    [SugarColumn(Length = 64, IsNullable = true, ColumnDescription = "负责人")]
    public string? Leader { get; set; }

    [SugarColumn(Length = 32, IsNullable = true, ColumnDescription = "联系电话")]
    public string? Phone { get; set; }

    /// <summary>
    /// 状态（1=正常 0=停用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}
