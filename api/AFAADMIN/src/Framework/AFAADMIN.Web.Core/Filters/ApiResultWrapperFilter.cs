using AFAADMIN.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AFAADMIN.Web.Core.Filters;

/// <summary>
/// 统一包装 Controller 返回结果为 ApiResult
/// </summary>
public class ApiResultWrapperFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // 跳过已经是 ApiResult 的、或文件下载等非 ObjectResult 的
        if (context.Result is ObjectResult objectResult)
        {
            // 如果返回值已经是 ApiResult 类型则不再包装
            if (objectResult.Value is ApiResult<object> || objectResult.Value is ApiResult)
                return;

            var apiResult = ApiResult<object>.Success(objectResult.Value);
            context.Result = new ObjectResult(apiResult) { StatusCode = 200 };
        }
    }

    public void OnResultExecuted(ResultExecutedContext context) { }
}
