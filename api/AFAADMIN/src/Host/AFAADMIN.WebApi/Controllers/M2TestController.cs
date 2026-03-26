using AFAADMIN.Common.Config;
using AFAADMIN.Common.Crypto;
using AFAADMIN.Common.Models;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SqlSugar;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// M2 阶段验证接口 — 数据库、加密、限流
/// </summary>
[Route("api/system/m2")]
public class M2TestController : ApiControllerBase
{
    private readonly ISqlSugarClient _db;
    private readonly SecurityConfig _securityConfig;
    private readonly ILogger<M2TestController> _logger;

    public M2TestController(ISqlSugarClient db, SecurityConfig securityConfig,
        ILogger<M2TestController> logger)
    {
        _db = db;
        _securityConfig = securityConfig;
        _logger = logger;
    }

    /// <summary>
    /// 测试数据库连接
    /// </summary>
    [HttpGet("db-check")]
    [Unencrypted]
    public IActionResult DbCheck()
    {
        try
        {
            var isConnected = _db.Ado.IsValidConnection();
            return Ok(ApiResult<object>.Success(new
            {
                Connected = isConnected,
                DbType = _db.CurrentConnectionConfig.DbType.ToString()
            }, isConnected ? "数据库连接成功" : "数据库连接失败"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据库连接测试失败");
            return Ok(ApiResult<object>.Fail($"数据库连接失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 测试 SM3 哈希
    /// </summary>
    [HttpGet("sm3")]
    [Unencrypted]
    public IActionResult TestSM3([FromQuery] string input = "admin123")
    {
        var salt = SM3Helper.GenerateSalt();
        var hash = SM3Helper.HashWithSalt(input, salt);
        var verified = SM3Helper.Verify(input, salt, hash);

        return Ok(ApiResult<object>.Success(new
        {
            Input = input,
            Salt = salt,
            Hash = hash,
            Verified = verified
        }));
    }

    /// <summary>
    /// 测试 SM4 加解密
    /// </summary>
    [HttpGet("sm4")]
    [Unencrypted]
    public IActionResult TestSM4([FromQuery] string input = "13800138000")
    {
        var sm4Key = _securityConfig.Encryption.SM4Key;
        if (string.IsNullOrEmpty(sm4Key))
        {
            return Ok(ApiResult.Fail("SM4Key 未配置，请检查 security.json"));
        }

        // ECB 模式（字段加密）
        var ecbEncrypted = SM4Helper.EncryptECB(input, sm4Key);
        var ecbDecrypted = SM4Helper.DecryptECB(ecbEncrypted, sm4Key);

        // CBC 模式（报文加密）
        var cbcEncrypted = SM4Helper.EncryptCBC(input, sm4Key);
        var cbcDecrypted = SM4Helper.DecryptCBC(cbcEncrypted, sm4Key);

        return Ok(ApiResult<object>.Success(new
        {
            Input = input,
            ECB = new { Encrypted = ecbEncrypted, Decrypted = ecbDecrypted },
            CBC = new { Encrypted = cbcEncrypted, Decrypted = cbcDecrypted }
        }));
    }

    /// <summary>
    /// 测试限流（标记 login 策略，频繁调用会触发 429）
    /// </summary>
    [HttpGet("rate-limit")]
    [Unencrypted]
    [EnableRateLimiting("login")]
    public IActionResult TestRateLimit()
    {
        return Ok(ApiResult.Success("请求成功，连续快速调用此接口可测试限流"));
    }
}
