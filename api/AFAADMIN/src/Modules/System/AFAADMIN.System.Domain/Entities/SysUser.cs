using AFAADMIN.Common.Attributes;
using AFAADMIN.Database.Entities;
using SqlSugar;

namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 系统用户实体
/// </summary>
[SugarTable("sys_user", "系统用户表")]
public class SysUser : BaseEntity
{
    /// <summary>
    /// 用户名（登录账号）
    /// </summary>
    [SugarColumn(Length = 64, IsNullable = false, ColumnDescription = "用户名")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(Length = 64, IsNullable = true, ColumnDescription = "昵称")]
    public string? NickName { get; set; }

    /// <summary>
    /// 密码（SM3 哈希后存储）
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = false, ColumnDescription = "密码")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 密码盐值
    /// </summary>
    [SugarColumn(Length = 64, IsNullable = false, ColumnDescription = "密码盐值")]
    public string Salt { get; set; } = string.Empty;

    /// <summary>
    /// 手机号（SM4 加密存储）
    /// </summary>
    [SensitiveField]
    [SugarColumn(Length = 256, IsNullable = true, ColumnDescription = "手机号")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = true, ColumnDescription = "邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 身份证号（SM4 加密存储）
    /// </summary>
    [SensitiveField]
    [SugarColumn(Length = 256, IsNullable = true, ColumnDescription = "身份证号")]
    public string? IdCard { get; set; }

    /// <summary>
    /// 头像 URL
    /// </summary>
    [SugarColumn(Length = 512, IsNullable = true, ColumnDescription = "头像")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 状态（1=正常 0=停用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}
