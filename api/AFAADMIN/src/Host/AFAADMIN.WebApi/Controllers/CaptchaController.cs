using AFAADMIN.Common.Models;
using AFAADMIN.Tools;
using AFAADMIN.Web.Core.Attributes;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 验证码接口
/// </summary>
[Route("api/captcha")]
public class CaptchaController : ApiControllerBase
{
    private readonly ICaptcha _captcha;

    public CaptchaController(ICaptcha captcha)
    {
        _captcha = captcha;
    }

    /// <summary>
    /// 获取图形验证码
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [Unencrypted]
    public IActionResult Get([FromQuery] string? id = null)
    {
        var captchaId = id ?? Guid.NewGuid().ToString("N");
        var captchaData = _captcha.Generate(captchaId);

        return Ok(ApiResult<object>.Success(new
        {
            Id = captchaId,
            Image = captchaData.Base64
        }));
    }

    /// <summary>
    /// 校验验证码
    /// </summary>
    [HttpPost("verify")]
    [AllowAnonymous]
    [Unencrypted]
    public IActionResult Verify([FromBody] CaptchaVerifyDto dto)
    {
        var valid = _captcha.Validate(dto.Id, dto.Code);
        if (valid)
            return Ok(ApiResult.Success("验证通过"));

        return Ok(ApiResult.Fail("验证码错误", 400));
    }

    /// <summary>
    /// 二维码生成
    /// </summary>
    [HttpGet("qrcode")]
    [AllowAnonymous]
    [Unencrypted]
    public IActionResult QrCode([FromQuery] string content)
    {
        if (string.IsNullOrEmpty(content))
            return Ok(ApiResult.Fail("内容不能为空"));

        var base64 = QrCodeHelper.GenerateBase64(content);
        return Ok(ApiResult<object>.Success(new { Image = base64 }));
    }
}

public class CaptchaVerifyDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
