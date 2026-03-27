namespace AFAADMIN.Web.Core.Attributes;

/// <summary>
/// 权限标记特性 — 标记需要特定权限才能访问的接口
/// 用法：[Permission("sys:user:add")]
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermissionAttribute : Attribute
{
    public string Code { get; }

    public PermissionAttribute(string code)
    {
        Code = code;
    }
}
