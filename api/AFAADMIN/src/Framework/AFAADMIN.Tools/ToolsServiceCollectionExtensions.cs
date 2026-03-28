using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFAADMIN.Tools;

public static class ToolsServiceCollectionExtensions
{
    /// <summary>
    /// 注册工具链服务
    /// </summary>
    public static IServiceCollection AddAfaTools(this IServiceCollection services,
        IConfiguration configuration)
    {
        // 验证码 — Lazy.Captcha 需要 IConfiguration 参数
        services.AddCaptcha(configuration);

        return services;
    }
}