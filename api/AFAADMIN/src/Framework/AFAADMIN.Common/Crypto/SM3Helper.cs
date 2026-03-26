using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;
using System.Text;

namespace AFAADMIN.Common.Crypto;

/// <summary>
/// SM3 国密哈希工具（用于密码存储）
/// </summary>
public static class SM3Helper
{
    /// <summary>
    /// SM3 哈希（无盐）
    /// </summary>
    public static string Hash(string input)
    {
        var data = Encoding.UTF8.GetBytes(input);
        var digest = new SM3Digest();
        digest.BlockUpdate(data, 0, data.Length);
        var result = new byte[digest.GetDigestSize()];
        digest.DoFinal(result, 0);
        return Convert.ToHexString(result).ToLower();
    }

    /// <summary>
    /// SM3 哈希 + 动态盐值（密码存储专用）
    /// </summary>
    public static string HashWithSalt(string input, string salt)
    {
        return Hash($"{salt}{input}{salt}");
    }

    /// <summary>
    /// 生成随机盐值（16 字节，32 位 Hex）
    /// </summary>
    public static string GenerateSalt()
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToHexString(saltBytes).ToLower();
    }

    /// <summary>
    /// 验证密码是否匹配
    /// </summary>
    public static bool Verify(string input, string salt, string hashedValue)
    {
        var hash = HashWithSalt(input, salt);
        return string.Equals(hash, hashedValue, StringComparison.OrdinalIgnoreCase);
    }
}
