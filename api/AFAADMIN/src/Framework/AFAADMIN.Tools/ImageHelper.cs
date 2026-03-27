using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace AFAADMIN.Tools;

/// <summary>
/// 图片处理工具（基于 ImageSharp）
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// 压缩图片
    /// </summary>
    /// <param name="input">输入流</param>
    /// <param name="quality">质量 1-100</param>
    /// <param name="format">输出格式: jpg/png/webp</param>
    public static async Task<MemoryStream> CompressAsync(Stream input, int quality = 75,
        string format = "jpg")
    {
        using var image = await Image.LoadAsync(input);
        var output = new MemoryStream();

        switch (format.ToLower())
        {
            case "png":
                await image.SaveAsPngAsync(output, new PngEncoder
                {
                    CompressionLevel = PngCompressionLevel.BestCompression
                });
                break;
            case "webp":
                await image.SaveAsWebpAsync(output, new WebpEncoder
                {
                    Quality = quality
                });
                break;
            default:
                await image.SaveAsJpegAsync(output, new JpegEncoder
                {
                    Quality = quality
                });
                break;
        }

        output.Seek(0, SeekOrigin.Begin);
        return output;
    }

    /// <summary>
    /// 缩放图片（等比例）
    /// </summary>
    public static async Task<MemoryStream> ResizeAsync(Stream input, int maxWidth, int maxHeight,
        string format = "jpg", int quality = 85)
    {
        using var image = await Image.LoadAsync(input);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(maxWidth, maxHeight),
            Mode = ResizeMode.Max
        }));

        var output = new MemoryStream();
        switch (format.ToLower())
        {
            case "png":
                await image.SaveAsPngAsync(output);
                break;
            case "webp":
                await image.SaveAsWebpAsync(output, new WebpEncoder { Quality = quality });
                break;
            default:
                await image.SaveAsJpegAsync(output, new JpegEncoder { Quality = quality });
                break;
        }

        output.Seek(0, SeekOrigin.Begin);
        return output;
    }

    /// <summary>
    /// 生成缩略图
    /// </summary>
    public static async Task<MemoryStream> ThumbnailAsync(Stream input, int size = 200)
    {
        return await ResizeAsync(input, size, size, "jpg", 80);
    }
}
