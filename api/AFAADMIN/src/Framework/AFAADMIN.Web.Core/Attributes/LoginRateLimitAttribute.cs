using Microsoft.AspNetCore.RateLimiting;

namespace AFAADMIN.Web.Core.Attributes;

/// <summary>
/// 标记使用登录接口限流策略
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LoginRateLimitAttribute : EnableRateLimitingAttribute
{
    public LoginRateLimitAttribute() : base(Extensions.RateLimitingExtensions.LoginPolicy) { }
}
