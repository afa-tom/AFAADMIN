using System.IdentityModel.Tokens.Jwt;
using AFAADMIN.Common.Cache;
using AFAADMIN.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.Web.Core.Middlewares;

/// <summary>
/// Token 黑名单中间件 — 在认证之后拦截已登出的 Token
/// </summary>
public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenBlacklistMiddleware> _logger;

    public TokenBlacklistMiddleware(RequestDelegate next,
        ILogger<TokenBlacklistMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICacheService cacheService)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    var jti = jwtToken.Id;
                    if (!string.IsNullOrEmpty(jti))
                    {
                        var isBlocked = await cacheService.ExistsAsync(CacheKeys.TokenBlacklist(jti));
                        if (isBlocked)
                        {
                            _logger.LogDebug("Token 已被加入黑名单: {Jti}", jti);
                            context.Response.StatusCode = 200;
                            context.Response.ContentType = "application/json";
                            var result = ApiResult.Fail("Token 已失效，请重新登录", 401);
                            await context.Response.WriteAsJsonAsync(result);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Token 黑名单检查异常，跳过");
            }
        }

        await _next(context);
    }
}
