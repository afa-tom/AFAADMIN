using AFAADMIN.Database.Config;
using AFAADMIN.Database.Encryption;
using AFAADMIN.Database.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace AFAADMIN.Database;

/// <summary>
/// 数据库模块服务注册
/// </summary>
public static class DatabaseServiceCollectionExtensions
{
    /// <summary>
    /// 注册 SqlSugar 多库支持 + 泛型仓储
    /// </summary>
    public static IServiceCollection AddAfaDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        // 绑定配置
        var dbConfig = new DatabaseConfig();
        configuration.GetSection("Database").Bind(dbConfig);
        services.AddSingleton(dbConfig);

        // 注册 SqlSugarClient（Scoped 生命周期，每次请求一个实例）
        services.AddScoped<ISqlSugarClient>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<SqlSugarClient>>();
            return SqlSugarFactory.CreateClient(dbConfig, logger);
        });

        // 注册泛型仓储
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // 注册敏感字段加解密服务
        services.AddSingleton(sp =>
        {
            var sm4Key = configuration["Security:Encryption:SM4Key"] ?? string.Empty;
            var logger = sp.GetRequiredService<ILogger<SensitiveFieldEncryptor>>();
            return new SensitiveFieldEncryptor(sm4Key, logger);
        });

        return services;
    }

    /// <summary>
    /// 初始化数据库（CodeFirst 建表，开发环境使用）
    /// </summary>
    public static void InitDatabase(this IServiceProvider serviceProvider, bool createTable = false)
    {
        if (!createTable) return;

        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SqlSugarClient>>();

        // 扫描所有继承 BaseEntity 的实体类型并建表
        var entityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.StartsWith("AFAADMIN"))
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return []; }
            })
            .Where(t => t.IsClass && !t.IsAbstract
                      && typeof(Entities.BaseEntity).IsAssignableFrom(t))
            .ToArray();

        if (entityTypes.Length > 0)
        {
            db.CodeFirst.InitTables(entityTypes);
            logger.LogInformation("CodeFirst 建表完成，共 {Count} 个实体", entityTypes.Length);
        }
    }
}
