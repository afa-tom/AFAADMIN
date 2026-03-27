using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AFAADMIN.Web.Core.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddAfaSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AFAADMIN API",
                Version = "v1",
                Description = "AFAADMIN 系统管理后台 API 文档"
            });

            // JWT Bearer 认证配置（预留）
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header. 示例: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
