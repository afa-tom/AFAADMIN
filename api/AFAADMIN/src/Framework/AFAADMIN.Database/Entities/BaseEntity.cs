using SqlSugar;

namespace AFAADMIN.Database.Entities;

/// <summary>
/// 实体基类 — 包含主键、审计字段、软删除
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键（雪花算法，M4 阶段替换为自定义雪花 ID）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnDescription = "主键")]
    public long Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 创建人 ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人", IsNullable = true)]
    public long? CreateBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 更新人 ID
    /// </summary>
    [SugarColumn(ColumnDescription = "更新人", IsNullable = true)]
    public long? UpdateBy { get; set; }

    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    [SugarColumn(ColumnDescription = "是否删除")]
    public bool IsDeleted { get; set; } = false;
}
