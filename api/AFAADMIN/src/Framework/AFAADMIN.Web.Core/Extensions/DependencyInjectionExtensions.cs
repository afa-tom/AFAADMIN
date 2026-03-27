using System.Reflection;
using AFAADMIN.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AFAADMIN.Web.Core.Extensions;

/// <summary>
/// 自动扫描并注册实现了标记接口的服务
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// 扫描所有已加载程序集，自动注册 DI 服务
    /// </summary>
    public static IServiceCollection AddAfaDependencies(this IServiceCollection services)
    {
        // 获取所有以 AFAADMIN 开头的程序集
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.StartsWith("AFAADMIN"))
            .ToList();

        // 也可主动加载未被引用的程序集
        var referencedPaths = Directory.GetFiles(AppContext.BaseDirectory, "AFAADMIN.*.dll");
        foreach (var path in referencedPaths)
        {
            var assemblyName = AssemblyName.GetAssemblyName(path);
            if (!assemblies.Any(a => a.FullName == assemblyName.FullName))
            {
                assemblies.Add(Assembly.Load(assemblyName));
            }
        }

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface);

            foreach (var type in types)
            {
                // 获取该类实现的所有接口（排除标记接口本身）
                var interfaces = type.GetInterfaces()
                    .Where(i => i != typeof(ITransientDependency)
                             && i != typeof(IScopedDependency)
                             && i != typeof(ISingletonDependency))
                    .ToList();

                if (typeof(ITransientDependency).IsAssignableFrom(type))
                {
                    RegisterService(services, type, interfaces, ServiceLifetime.Transient);
                }
                else if (typeof(IScopedDependency).IsAssignableFrom(type))
                {
                    RegisterService(services, type, interfaces, ServiceLifetime.Scoped);
                }
                else if (typeof(ISingletonDependency).IsAssignableFrom(type))
                {
                    RegisterService(services, type, interfaces, ServiceLifetime.Singleton);
                }
            }
        }

        return services;
    }

    private static void RegisterService(IServiceCollection services, Type implementationType,
        List<Type> interfaces, ServiceLifetime lifetime)
    {
        if (interfaces.Count == 0)
        {
            // 没有业务接口，直接注册自身
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
        }
        else
        {
            // 为每个接口注册
            foreach (var interfaceType in interfaces)
            {
                services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
            }
        }
    }
}
