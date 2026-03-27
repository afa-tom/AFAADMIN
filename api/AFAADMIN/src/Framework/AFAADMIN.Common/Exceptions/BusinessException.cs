namespace AFAADMIN.Common.Exceptions;

/// <summary>
/// 自定义业务异常
/// </summary>
public class BusinessException : Exception
{
    public int Code { get; }
    public BusinessException(string message, int code = 400) : base(message)
    {
        Code = code;
    }
}
