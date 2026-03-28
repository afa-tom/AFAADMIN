using AFAADMIN.Common.Config;
using AfaMinioConfig = AFAADMIN.Common.Config.MinioConfig;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace AFAADMIN.Storage.Providers;

/// <summary>
/// MinIO 对象存储实现
/// </summary>
public class MinioStorageProvider : IStorageProvider
{
    private readonly IMinioClient _client;
    private readonly AfaMinioConfig _config;
    private readonly ILogger<MinioStorageProvider> _logger;

    public MinioStorageProvider(StorageConfig storageConfig, ILogger<MinioStorageProvider> logger)
    {
        _config = storageConfig.Minio;
        _logger = logger;

        _client = new MinioClient()
            .WithEndpoint(_config.Endpoint)
            .WithCredentials(_config.AccessKey, _config.SecretKey)
            .WithSSL(_config.UseSSL)
            .Build();
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string? directory = null)
    {
        await EnsureBucketAsync();

        var dir = directory ?? DateTime.Now.ToString("yyyy/MM/dd");
        var ext = Path.GetExtension(fileName);
        var objectName = $"{dir}/{Guid.NewGuid():N}{ext}";

        var contentType = GetContentType(ext);

        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_config.BucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType));

        _logger.LogDebug("文件已上传到 MinIO: {Object}", objectName);
        return objectName;
    }

    public async Task<Stream?> DownloadAsync(string filePath)
    {
        try
        {
            var ms = new MemoryStream();
            await _client.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_config.BucketName)
                .WithObject(filePath)
                .WithCallbackStream(stream => stream.CopyTo(ms)));

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MinIO 下载失败: {Path}", filePath);
            return null;
        }
    }

    public string GetUrl(string filePath)
    {
        var scheme = _config.UseSSL ? "https" : "http";
        return $"{scheme}://{_config.Endpoint}/{_config.BucketName}/{filePath}";
    }

    public async Task<bool> DeleteAsync(string filePath)
    {
        try
        {
            await _client.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_config.BucketName)
                .WithObject(filePath));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MinIO 删除失败: {Path}", filePath);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string filePath)
    {
        try
        {
            await _client.StatObjectAsync(new StatObjectArgs()
                .WithBucket(_config.BucketName)
                .WithObject(filePath));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task EnsureBucketAsync()
    {
        var exists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_config.BucketName));
        if (!exists)
        {
            await _client.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_config.BucketName));
            _logger.LogInformation("MinIO Bucket 已创建: {Bucket}", _config.BucketName);
        }
    }

    private static string GetContentType(string ext)
    {
        return ext.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".zip" => "application/zip",
            ".mp4" => "video/mp4",
            _ => "application/octet-stream"
        };
    }
}
