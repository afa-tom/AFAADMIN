using AFAADMIN.Common.Models;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Application.Services;
using AFAADMIN.Web.Core.Attributes;
using AFAADMIN.Web.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 认证接口
/// </summary>
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 登录
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [Unencrypted]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(ApiResult<LoginResultDto>.Success(result, "登录成功"));
    }

    /// <summary>
    /// 刷新 Token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [Unencrypted]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        return Ok(ApiResult<LoginResultDto>.Success(result));
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    [HttpGet("userinfo")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = HttpContext.GetUserId();
        var result = await _authService.GetCurrentUserAsync(userId);
        return Ok(ApiResult<CurrentUserDto>.Success(result));
    }

    /// <summary>
    /// 登出（Token 加入黑名单）
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var authHeader = HttpContext.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            await _authService.LogoutAsync(token);
        }
        return Ok(ApiResult.Success("已登出"));
    }
}
