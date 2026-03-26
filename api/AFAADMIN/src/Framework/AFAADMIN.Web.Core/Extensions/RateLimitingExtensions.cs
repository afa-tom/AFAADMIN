using System.Threading.RateLimiting;
using AFAADMIN.Common.Config;
using AFAADMIN.Common.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFAADMIN.Web.Core.Extensions;

/// <summary>
/// .NET 8 内置限流配置
/// </summary>
public static class RateLimitingExtensions
{
    public const string GlobalPolicy = "global";
    public const string LoginPolicy = "login";

    /// <summary>
    /// 注册限流服务
    /// </summary>
    public static IServiceCollection AddAfaRateLimiting(this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = new RateLimitingConfig();
        configuration.GetSection("Security:RateLimiting").Bind(config);

        if (!config.EnableRateLimiting) return services;

        services.AddRateLimiter(options =>
        {
            // 全局策略：基于 IP 的固定窗口限流
            options.AddFixedWindowLimiter(GlobalPolicy, opt =>
            {
                opt.PermitLimit = config.PermitLimit;
                opt.Window = TimeSpan.FromSeconds(config.WindowSeconds);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 2;
            });

            // 登录接口策略：更严格的限流
            options.AddFixedWindowLimiter(LoginPolicy, opt =>
            {
                opt.PermitLimit = config.LoginPermitLimit;
                opt.Window = TimeSpan.FromSeconds(config.LoginWindowSeconds);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });

            // 被限流时的响应
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                context.HttpContext.Response.ContentType = "application/json";

                var result = ApiResult.Fail("请求过于频繁，请稍后再试", 429);
                await context.HttpContext.Response.WriteAsJsonAsync(result, token);
            };
        });

        return services;
    }

    /// <summary>
    /// 启用限流中间件
    /// </summary>
    public static IApplicationBuilder UseAfaRateLimiting(this IApplicationBuilder app,
        IConfiguration configuration)
    {
        var enabled = configuration.GetValue<bool>("Security:RateLimiting:EnableRateLimiting");
        if (enabled)
        {
            app.UseRateLimiter();
        }
        return app;
    }
}
