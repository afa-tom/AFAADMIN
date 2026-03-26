using AFAADMIN.Common.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFAADMIN.Web.Core.Extensions;

public static class SecurityExtensions
{
    /// <summary>
    /// 注册安全配置（JWT、加密、限流配置模型）
    /// </summary>
    public static IServiceCollection AddAfaSecurity(this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = new SecurityConfig();
        configuration.GetSection("Security").Bind(config);
        services.AddSingleton(config);

        return services;
    }
}
