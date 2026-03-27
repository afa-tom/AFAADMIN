namespace AFAADMIN.Storage;

/// <summary>
/// 文件上传结果
/// </summary>
public class FileUploadResult
{
    /// <summary>
    /// 存储路径标识
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 访问 URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 原始文件名
    /// </summary>
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    public string Extension { get; set; } = string.Empty;
}
