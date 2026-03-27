using AFAADMIN.Database.Entities;
using SqlSugar;

namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 字典类型
/// </summary>
[SugarTable("sys_dict_type", "字典类型表")]
public class SysDictType : BaseEntity
{
    [SugarColumn(Length = 64, IsNullable = false, ColumnDescription = "字典名称")]
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典编码（唯一标识，如 sys_user_status）
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = false, ColumnDescription = "字典编码")]
    public string DictCode { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}
