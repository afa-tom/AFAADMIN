using AFAADMIN.Database.Entities;
using SqlSugar;

namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 字典数据
/// </summary>
[SugarTable("sys_dict_data", "字典数据表")]
public class SysDictData : BaseEntity
{
    /// <summary>
    /// 关联的字典类型 ID
    /// </summary>
    [SugarColumn(ColumnDescription = "字典类型ID")]
    public long DictTypeId { get; set; }

    [SugarColumn(Length = 128, IsNullable = false, ColumnDescription = "字典标签")]
    public string DictLabel { get; set; } = string.Empty;

    [SugarColumn(Length = 128, IsNullable = false, ColumnDescription = "字典键值")]
    public string DictValue { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// CSS 样式（标签颜色等）
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = true, ColumnDescription = "样式")]
    public string? CssClass { get; set; }

    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}
