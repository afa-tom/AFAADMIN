using Microsoft.Extensions.DependencyInjection;

namespace AFAADMIN.System.Infrastructure;

/// <summary>
/// System 模块基础设施注册
/// </summary>
public static class SystemInfrastructureExtensions
{
    /// <summary>
    /// 初始化 System 模块（建表 + 种子数据）
    /// </summary>
    public static void InitSystemModule(this IServiceProvider serviceProvider)
    {
        SeedDataInitializer.SeedData(serviceProvider);
    }
}
