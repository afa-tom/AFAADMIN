using AFAADMIN.Common.Exceptions;
using AFAADMIN.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.Web.Core.Filters;

/// <summary>
/// 全局异常过滤器：捕获所有未处理异常，转换为友好的 ApiResult 返回
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        ApiResult result;

        switch (context.Exception)
        {
            // 业务异常 - 返回业务错误码和消息
            case BusinessException bizEx:
                result = ApiResult.Fail(bizEx.Message, bizEx.Code);
                _logger.LogWarning("业务异常: {Message}", bizEx.Message);
                break;

            // 未授权
            case UnauthorizedAccessException:
                result = ApiResult.Fail("未授权访问", 401);
                _logger.LogWarning("未授权访问: {Path}", context.HttpContext.Request.Path);
                break;

            // 其他未知异常
            default:
                result = ApiResult.Fail("服务器内部错误，请稍后重试", 500);
                _logger.LogError(context.Exception, "未处理异常: {Message}", context.Exception.Message);
                break;
        }

        context.Result = new ObjectResult(result)
        {
            StatusCode = 200  // HTTP 状态码统一 200，业务码通过 Code 字段区分
        };
        context.ExceptionHandled = true;
    }
}
