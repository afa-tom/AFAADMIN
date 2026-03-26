using AFAADMIN.Common.Enums;
using AFAADMIN.Database.Config;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace AFAADMIN.Database;

/// <summary>
/// SqlSugar 客户端工厂 — 根据配置动态创建对应数据库的 SqlSugarClient
/// </summary>
public static class SqlSugarFactory
{
    /// <summary>
    /// 将自定义 DbTypeEnum 映射为 SqlSugar 的 DbType
    /// </summary>
    public static DbType MapDbType(DbTypeEnum dbType)
    {
        return dbType switch
        {
            DbTypeEnum.MySql => DbType.MySql,
            DbTypeEnum.SqlServer => DbType.SqlServer,
            DbTypeEnum.PostgreSql => DbType.PostgreSQL,
            DbTypeEnum.Sqlite => DbType.Sqlite,
            DbTypeEnum.Oracle => DbType.Oracle,
            _ => throw new NotSupportedException($"不支持的数据库类型: {dbType}")
        };
    }

    /// <summary>
    /// 创建 SqlSugarClient 实例
    /// </summary>
    public static SqlSugarClient CreateClient(DatabaseConfig config, ILogger? logger = null)
    {
        var client = new SqlSugarClient(new ConnectionConfig
        {
            DbType = MapDbType(config.DbType),
            ConnectionString = config.ConnectionString,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            MoreSettings = new ConnMoreSettings
            {
                IsAutoRemoveDataCache = true
            }
        });

        // ===== AOP: SQL 日志 =====
        if (config.EnableSqlLog && logger != null)
        {
            client.Aop.OnLogExecuting = (sql, pars) =>
            {
                var parameters = string.Join(", ",
                    pars?.Select(p => $"{p.ParameterName}={p.Value}") ?? []);
                logger.LogDebug("【SQL】{Sql} | 参数: {Parameters}", sql, parameters);
            };

            client.Aop.OnLogExecuted = (sql, pars) =>
            {
                logger.LogDebug("【SQL耗时】{Time}ms", client.Ado.SqlExecutionTime.TotalMilliseconds);
            };

            client.Aop.OnError = (exp) =>
            {
                logger.LogError(exp, "【SQL异常】{Message}", exp.Message);
            };
        }

        // ===== AOP: 审计字段自动填充 =====
        if (config.EnableAudit)
        {
            client.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                // 插入操作
                if (entityInfo.OperationType == DataFilterType.InsertByObject)
                {
                    if (entityInfo.PropertyName == nameof(Entities.BaseEntity.CreateTime))
                    {
                        entityInfo.SetValue(DateTime.Now);
                    }
                    if (entityInfo.PropertyName == nameof(Entities.BaseEntity.IsDeleted))
                    {
                        entityInfo.SetValue(false);
                    }
                }
                // 更新操作
                if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                {
                    if (entityInfo.PropertyName == nameof(Entities.BaseEntity.UpdateTime))
                    {
                        entityInfo.SetValue(DateTime.Now);
                    }
                }
            };
        }

        // ===== 全局过滤器: 软删除 =====
        client.QueryFilter.AddTableFilter<Entities.BaseEntity>(it => it.IsDeleted == false);

        return client;
    }
}
