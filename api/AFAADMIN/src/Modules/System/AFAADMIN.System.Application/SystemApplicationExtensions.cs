using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AFAADMIN.System.Application;

/// <summary>
/// System 模块 Application 层注册
/// </summary>
public static class SystemApplicationExtensions
{
    public static IServiceCollection AddSystemApplication(this IServiceCollection services)
    {
        // 注册 FluentValidation 校验器（自动扫描当前程序集）
        services.AddValidatorsFromAssembly(typeof(SystemApplicationExtensions).Assembly);

        return services;
    }
}
