using AFAADMIN.Web.Core.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace AFAADMIN.Web.Core.Extensions;

public static class EncryptionMiddlewareExtensions
{
    /// <summary>
    /// 启用 API 报文全局加解密中间件
    /// </summary>
    public static IApplicationBuilder UseAfaEncryption(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AfaEncryptionMiddleware>();
    }
}
