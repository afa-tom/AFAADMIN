using AFAADMIN.Common.Exceptions;
using AFAADMIN.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 系统基础接口 - 用于验证架构运行状态
/// </summary>
public class SystemController : ApiControllerBase
{
    private readonly ILogger<SystemController> _logger;
    private readonly IConfiguration _configuration;

    public SystemController(ILogger<SystemController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// 服务心跳检测
    /// </summary>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            Service = _configuration["Application:Name"] ?? "AFAADMIN",
            Version = _configuration["Application:Version"] ?? "1.0.0",
            Time = DateTime.Now,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        });
    }

    /// <summary>
    /// 测试统一响应格式 - 成功
    /// </summary>
    [HttpGet("test/success")]
    public IActionResult TestSuccess()
    {
        _logger.LogInformation("测试成功响应");
        return Ok(ApiResult<string>.Success("Hello AFAADMIN!"));
    }

    /// <summary>
    /// 测试统一响应格式 - 业务异常
    /// </summary>
    [HttpGet("test/biz-error")]
    public IActionResult TestBusinessError()
    {
        throw new BusinessException("这是一个业务异常测试", 400);
    }

    /// <summary>
    /// 测试统一响应格式 - 未知异常（由全局过滤器捕获）
    /// </summary>
    [HttpGet("test/error")]
    public IActionResult TestError()
    {
        throw new InvalidOperationException("模拟的未知异常");
    }

    /// <summary>
    /// 测试配置文件加载 - 验证多配置分离是否生效
    /// </summary>
    [HttpGet("test/config")]
    public IActionResult TestConfig()
    {
        return Ok(ApiResult<object>.Success(new
        {
            DbType = _configuration["Database:DbType"],
            RedisInstance = _configuration["Redis:InstanceName"],
            JwtIssuer = _configuration["Security:Jwt:Issuer"],
            EnableEncryption = _configuration["Security:Encryption:EnableGlobalEncryption"],
            RateLimitEnabled = _configuration["Security:RateLimiting:EnableRateLimiting"]
        }));
    }
}
