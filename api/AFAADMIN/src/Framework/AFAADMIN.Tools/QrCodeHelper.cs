using QRCoder;

namespace AFAADMIN.Tools;

/// <summary>
/// 二维码工具
/// </summary>
public static class QrCodeHelper
{
    /// <summary>
    /// 生成二维码 Base64 图片
    /// </summary>
    public static string GenerateBase64(string content, int pixelPerModule = 10)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
        using var code = new PngByteQRCode(data);
        var bytes = code.GetGraphic(pixelPerModule);
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }

    /// <summary>
    /// 生成二维码字节
    /// </summary>
    public static byte[] GenerateBytes(string content, int pixelPerModule = 10)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
        using var code = new PngByteQRCode(data);
        return code.GetGraphic(pixelPerModule);
    }
}
