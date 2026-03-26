namespace AFAADMIN.Web.Core.Attributes;

/// <summary>
/// 标记不需要进行 API 报文加解密的接口
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class UnencryptedAttribute : Attribute { }
