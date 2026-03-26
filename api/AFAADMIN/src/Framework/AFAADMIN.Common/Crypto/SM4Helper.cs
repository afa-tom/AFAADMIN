using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;
using System.Text;

namespace AFAADMIN.Common.Crypto;

/// <summary>
/// SM4 国密对称加解密工具（ECB 用于字段加密，CBC 用于报文加密）
/// </summary>
public static class SM4Helper
{
    private const int KeySize = 16;  // 128 位
    private const int IvSize = 16;

    #region ECB 模式（敏感字段加密，确定性，同一明文产生同一密文）

    /// <summary>
    /// SM4-ECB 加密（字段级加密）
    /// </summary>
    public static string EncryptECB(string plainText, string hexKey)
    {
        var keyBytes = HexToBytes(hexKey);
        ValidateKey(keyBytes);

        var inputBytes = Encoding.UTF8.GetBytes(plainText);
        var cipher = CreateCipher(true, new KeyParameter(keyBytes), useCbc: false);
        var output = ProcessCipher(cipher, inputBytes);

        return Convert.ToBase64String(output);
    }

    /// <summary>
    /// SM4-ECB 解密（字段级解密）
    /// </summary>
    public static string DecryptECB(string cipherText, string hexKey)
    {
        var keyBytes = HexToBytes(hexKey);
        ValidateKey(keyBytes);

        var inputBytes = Convert.FromBase64String(cipherText);
        var cipher = CreateCipher(false, new KeyParameter(keyBytes), useCbc: false);
        var output = ProcessCipher(cipher, inputBytes);

        return Encoding.UTF8.GetString(output);
    }

    #endregion

    #region CBC 模式（API 报文加密，随机 IV，更安全）

    /// <summary>
    /// SM4-CBC 加密（报文级加密，返回 Base64(IV + CipherText)）
    /// </summary>
    public static string EncryptCBC(string plainText, string hexKey)
    {
        var keyBytes = HexToBytes(hexKey);
        ValidateKey(keyBytes);

        var iv = RandomNumberGenerator.GetBytes(IvSize);
        var inputBytes = Encoding.UTF8.GetBytes(plainText);
        var cipher = CreateCipher(true,
            new ParametersWithIV(new KeyParameter(keyBytes), iv), useCbc: true);
        var encrypted = ProcessCipher(cipher, inputBytes);

        // IV + 密文拼接
        var result = new byte[iv.Length + encrypted.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// SM4-CBC 解密（拆分 IV + CipherText）
    /// </summary>
    public static string DecryptCBC(string cipherText, string hexKey)
    {
        var keyBytes = HexToBytes(hexKey);
        ValidateKey(keyBytes);

        var allBytes = Convert.FromBase64String(cipherText);
        if (allBytes.Length < IvSize)
            throw new ArgumentException("密文长度不合法");

        var iv = new byte[IvSize];
        var encrypted = new byte[allBytes.Length - IvSize];
        Buffer.BlockCopy(allBytes, 0, iv, 0, IvSize);
        Buffer.BlockCopy(allBytes, IvSize, encrypted, 0, encrypted.Length);

        var cipher = CreateCipher(false,
            new ParametersWithIV(new KeyParameter(keyBytes), iv), useCbc: true);
        var output = ProcessCipher(cipher, encrypted);

        return Encoding.UTF8.GetString(output);
    }

    #endregion

    #region 内部方法

    private static IBufferedCipher CreateCipher(bool forEncryption,
        ICipherParameters parameters, bool useCbc)
    {
        IBlockCipher engine = new SM4Engine();
        IBlockCipherPadding padding = new Pkcs7Padding();

        BufferedBlockCipher cipher;
        if (useCbc)
            cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), padding);
        else
            cipher = new PaddedBufferedBlockCipher(engine, padding);

        cipher.Init(forEncryption, parameters);
        return cipher;
    }

    private static byte[] ProcessCipher(IBufferedCipher cipher, byte[] input)
    {
        var output = new byte[cipher.GetOutputSize(input.Length)];
        var len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
        len += cipher.DoFinal(output, len);

        // 裁剪到实际长度
        var result = new byte[len];
        Buffer.BlockCopy(output, 0, result, 0, len);
        return result;
    }

    private static byte[] HexToBytes(string hex)
    {
        return Convert.FromHexString(hex);
    }

    private static void ValidateKey(byte[] key)
    {
        if (key.Length != KeySize)
            throw new ArgumentException($"SM4 密钥必须为 {KeySize} 字节（{KeySize * 2} 位 Hex），当前: {key.Length} 字节");
    }

    #endregion
}
