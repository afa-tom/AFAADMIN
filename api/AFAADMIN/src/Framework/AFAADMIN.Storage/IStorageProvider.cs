namespace AFAADMIN.Storage;

/// <summary>
/// 文件存储统一接口
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="stream">文件流</param>
    /// <param name="fileName">文件名</param>
    /// <param name="directory">子目录（可选）</param>
    /// <returns>存储后的文件路径标识</returns>
    Task<string> UploadAsync(Stream stream, string fileName, string? directory = null);

    /// <summary>
    /// 下载文件
    /// </summary>
    Task<Stream?> DownloadAsync(string filePath);

    /// <summary>
    /// 获取文件访问 URL
    /// </summary>
    string GetUrl(string filePath);

    /// <summary>
    /// 删除文件
    /// </summary>
    Task<bool> DeleteAsync(string filePath);

    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    Task<bool> ExistsAsync(string filePath);
}
