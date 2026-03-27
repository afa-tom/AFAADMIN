using System.Text;
using AFAADMIN.Common.Config;
using AFAADMIN.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AFAADMIN.Web.Core.Authentication;

public static class JwtExtensions
{
    public static IServiceCollection AddAfaJwtAuth(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();
        configuration.GetSection("Security:Jwt").Bind(jwtConfig);

        // 注册 JwtService
        services.AddSingleton<IJwtService, JwtService>();

        // 注册 JWT 认证
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtConfig.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            // 自定义认证失败响应
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    var result = ApiResult.Fail("未登录或Token已过期", 401);
                    await context.Response.WriteAsJsonAsync(result);
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    var result = ApiResult.Fail("无权访问", 403);
                    await context.Response.WriteAsJsonAsync(result);
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}
