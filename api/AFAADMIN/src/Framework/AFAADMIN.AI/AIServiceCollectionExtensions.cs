using AFAADMIN.AI.Config;
using AFAADMIN.AI.Functions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace AFAADMIN.AI;

/// <summary>
/// AI Copilot 模块服务注册
/// </summary>
public static class AIServiceCollectionExtensions
{
    /// <summary>
    /// 注册 AI Copilot 服务（Semantic Kernel + Native Functions）
    /// </summary>
    public static IServiceCollection AddAfaAI(this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = new AIConfig();
        configuration.GetSection("AI").Bind(config);
        services.AddSingleton(config);

        if (!config.Enabled || string.IsNullOrEmpty(config.ApiKey))
        {
            // AI 未启用或未配置，注册空 Kernel
            services.AddSingleton<Kernel>(sp => new Kernel());
            return services;
        }

        // 注册 Semantic Kernel
        services.AddSingleton<Kernel>(sp =>
        {
            var builder = Kernel.CreateBuilder();

            // 根据 Provider 注册不同的 Chat Completion 服务
            switch (config.Provider.ToLower())
            {
                case "azureopenai":
                    builder.AddAzureOpenAIChatCompletion(
                        deploymentName: config.ModelId,
                        endpoint: config.Endpoint,
                        apiKey: config.ApiKey);
                    break;
                case "openai":
                default:
                    if (!string.IsNullOrEmpty(config.Endpoint))
                    {
                        // 自定义 Endpoint（兼容第三方 OpenAI 兼容 API）
                        builder.AddOpenAIChatCompletion(
                            modelId: config.ModelId,
                            apiKey: config.ApiKey,
                            httpClient: new HttpClient
                            {
                                BaseAddress = new Uri(config.Endpoint)
                            });
                    }
                    else
                    {
                        builder.AddOpenAIChatCompletion(
                            modelId: config.ModelId,
                            apiKey: config.ApiKey);
                    }
                    break;
            }

            // 注册 Native Functions
            var functions = new SystemQueryFunctions(sp);
            builder.Plugins.AddFromObject(functions, "SystemQuery");

            builder.Services.AddLogging(lb =>
                lb.AddConsole().SetMinimumLevel(LogLevel.Warning));

            return builder.Build();
        });

        return services;
    }
}
