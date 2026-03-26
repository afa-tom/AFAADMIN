namespace AFAADMIN.Common.Attributes;

/// <summary>
/// 标记需要自动 SM4 加解密的敏感字段（如手机号、身份证号）
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SensitiveFieldAttribute : Attribute { }
