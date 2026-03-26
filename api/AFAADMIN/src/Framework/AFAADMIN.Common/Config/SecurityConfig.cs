namespace AFAADMIN.Common.Config;

/// <summary>
/// 安全配置（映射 configs/security.json）
/// </summary>
public class SecurityConfig
{
    public JwtConfig Jwt { get; set; } = new();
    public EncryptionConfig Encryption { get; set; } = new();
    public RateLimitingConfig RateLimiting { get; set; } = new();
}

public class JwtConfig
{
    public string Issuer { get; set; } = "AFAADMIN";
    public string Audience { get; set; } = "AFAADMIN-Client";
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpireMinutes { get; set; } = 120;
    public int RefreshTokenExpireDays { get; set; } = 7;
}

public class EncryptionConfig
{
    /// <summary>
    /// 是否启用 API 报文全局加解密
    /// </summary>
    public bool EnableGlobalEncryption { get; set; } = false;

    /// <summary>
    /// SM4 密钥（32 位 Hex 字符串 = 16 字节）
    /// </summary>
    public string SM4Key { get; set; } = string.Empty;

    /// <summary>
    /// SM3 全局盐值（配合密码哈希使用）
    /// </summary>
    public string SM3Salt { get; set; } = string.Empty;
}

public class RateLimitingConfig
{
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>
    /// 全局：窗口时间内允许的最大请求数
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// 全局：滑动窗口时间（秒）
    /// </summary>
    public int WindowSeconds { get; set; } = 60;

    /// <summary>
    /// 登录接口：窗口时间内允许的最大请求数
    /// </summary>
    public int LoginPermitLimit { get; set; } = 5;

    /// <summary>
    /// 登录接口：窗口时间（秒）
    /// </summary>
    public int LoginWindowSeconds { get; set; } = 60;
}
