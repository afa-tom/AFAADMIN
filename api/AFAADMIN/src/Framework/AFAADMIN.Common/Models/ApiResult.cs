namespace AFAADMIN.Common.Models;

/// <summary>
/// 统一 API 响应模型
/// </summary>
public class ApiResult<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static ApiResult<T> Success(T? data = default, string message = "操作成功")
        => new() { Code = 200, Message = message, Data = data };

    public static ApiResult<T> Fail(string message = "操作失败", int code = 500)
        => new() { Code = code, Message = message };
}

public class ApiResult : ApiResult<object>
{
    public new static ApiResult Success(string message = "操作成功")
        => new() { Code = 200, Message = message };

    public new static ApiResult Fail(string message = "操作失败", int code = 500)
        => new() { Code = code, Message = message };
}
