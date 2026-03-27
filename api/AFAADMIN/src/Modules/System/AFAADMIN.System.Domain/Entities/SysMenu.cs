using AFAADMIN.Database.Entities;
using SqlSugar;

namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 系统菜单/权限
/// </summary>
[SugarTable("sys_menu", "系统菜单表")]
public class SysMenu : BaseEntity
{
    /// <summary>
    /// 父级 ID（0 表示顶级）
    /// </summary>
    [SugarColumn(ColumnDescription = "父级ID")]
    public long ParentId { get; set; } = 0;

    [SugarColumn(Length = 64, IsNullable = false, ColumnDescription = "菜单名称")]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型（1=目录 2=菜单 3=按钮）
    /// </summary>
    [SugarColumn(ColumnDescription = "菜单类型")]
    public int MenuType { get; set; } = 1;

    /// <summary>
    /// 权限标识，如 sys:user:add
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = true, ColumnDescription = "权限标识")]
    public string? Permission { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    [SugarColumn(Length = 256, IsNullable = true, ColumnDescription = "路由地址")]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [SugarColumn(Length = 256, IsNullable = true, ColumnDescription = "组件路径")]
    public string? Component { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = true, ColumnDescription = "图标")]
    public string? Icon { get; set; }

    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否可见
    /// </summary>
    [SugarColumn(ColumnDescription = "是否可见")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// 状态（1=正常 0=停用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}
