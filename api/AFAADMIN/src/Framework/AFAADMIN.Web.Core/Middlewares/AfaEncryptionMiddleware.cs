using AFAADMIN.Common.Config;
using AFAADMIN.Common.Crypto;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AFAADMIN.Web.Core.Middlewares;

/// <summary>
/// API 报文全局加解密中间件
/// - 拦截 Request：读取加密报文 → SM4-CBC 解密 → 转为明文 JSON 供 Controller 读取
/// - 拦截 Response：将响应 JSON → SM4-CBC 加密 → 写入加密流
/// - 标记 [Unencrypted] 的接口跳过
/// </summary>
public class AfaEncryptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityConfig _config;
    private readonly ILogger<AfaEncryptionMiddleware> _logger;

    public AfaEncryptionMiddleware(RequestDelegate next, SecurityConfig config,
        ILogger<AfaEncryptionMiddleware> logger)
    {
        _next = next;
        _config = config;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 未启用全局加密，直接跳过
        if (!_config.Encryption.EnableGlobalEncryption)
        {
            await _next(context);
            return;
        }

        // 检查端点是否标记了 [Unencrypted]
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<UnencryptedAttribute>() != null)
        {
            await _next(context);
            return;
        }

        var sm4Key = _config.Encryption.SM4Key;
        if (string.IsNullOrEmpty(sm4Key))
        {
            _logger.LogWarning("全局加密已启用但 SM4Key 未配置，跳过加解密");
            await _next(context);
            return;
        }

        // ===== 解密 Request Body =====
        if (HasBody(context.Request))
        {
            try
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var encryptedBody = await reader.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(encryptedBody))
                {
                    var decrypted = SM4Helper.DecryptCBC(encryptedBody.Trim('"'), sm4Key);
                    var decryptedBytes = Encoding.UTF8.GetBytes(decrypted);
                    context.Request.Body = new MemoryStream(decryptedBytes);
                    context.Request.ContentLength = decryptedBytes.Length;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Request 解密失败");
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("{\"code\":400,\"message\":\"请求报文解密失败\"}");
                return;
            }
        }

        // ===== 拦截 Response Body =====
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // 加密 Response
        try
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(responseText))
            {
                var encrypted = SM4Helper.EncryptCBC(responseText, sm4Key);
                var encryptedBytes = Encoding.UTF8.GetBytes($"\"{encrypted}\"");

                context.Response.ContentLength = encryptedBytes.Length;
                context.Response.ContentType = "application/json";
                await originalBodyStream.WriteAsync(encryptedBytes);
            }
            else
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Response 加密失败，返回原始响应");
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private static bool HasBody(HttpRequest request)
    {
        return request.ContentLength > 0
            || (request.ContentType != null
                && (request.Method == HttpMethods.Post
                    || request.Method == HttpMethods.Put
                    || request.Method == HttpMethods.Patch));
    }
}
