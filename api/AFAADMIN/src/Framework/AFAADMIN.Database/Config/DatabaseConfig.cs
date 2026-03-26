using AFAADMIN.Common.Enums;

namespace AFAADMIN.Database.Config;

/// <summary>
/// 数据库配置（映射 configs/database.json 中的 Database 节点）
/// </summary>
public class DatabaseConfig
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public DbTypeEnum DbType { get; set; } = DbTypeEnum.MySql;

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用 SQL 日志输出（开发环境建议开启）
    /// </summary>
    public bool EnableSqlLog { get; set; } = true;

    /// <summary>
    /// 是否启用审计字段自动填充
    /// </summary>
    public bool EnableAudit { get; set; } = true;
}
