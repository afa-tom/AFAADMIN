using Microsoft.Extensions.DependencyInjection;

namespace AFAADMIN.EventBus;

public static class EventBusServiceCollectionExtensions
{
    /// <summary>
    /// 注册 MediatR 事件总线（自动扫描所有 AFAADMIN 程序集中的 Handler）
    /// </summary>
    public static IServiceCollection AddAfaEventBus(this IServiceCollection services)
    {
        // 扫描所有 AFAADMIN.* 程序集
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.StartsWith("AFAADMIN"))
            .ToArray();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });

        return services;
    }
}
