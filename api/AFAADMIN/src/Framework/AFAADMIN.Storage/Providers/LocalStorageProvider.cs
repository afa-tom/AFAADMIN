using AFAADMIN.Common.Config;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.Storage.Providers;

/// <summary>
/// 本地文件存储实现
/// </summary>
public class LocalStorageProvider : IStorageProvider
{
    private readonly StorageConfig _config;
    private readonly ILogger<LocalStorageProvider> _logger;

    public LocalStorageProvider(StorageConfig config, ILogger<LocalStorageProvider> logger)
    {
        _config = config;
        _logger = logger;

        // 确保根目录存在
        var basePath = GetBasePath();
        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string? directory = null)
    {
        var dir = directory ?? DateTime.Now.ToString("yyyy/MM/dd");
        var ext = Path.GetExtension(fileName);
        var newName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine(dir, newName).Replace("\\", "/");
        var fullPath = Path.Combine(GetBasePath(), relativePath);

        var fullDir = Path.GetDirectoryName(fullPath)!;
        if (!Directory.Exists(fullDir))
            Directory.CreateDirectory(fullDir);

        await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fs);

        _logger.LogDebug("文件已保存到本地: {Path}", relativePath);
        return relativePath;
    }

    public Task<Stream?> DownloadAsync(string filePath)
    {
        var fullPath = Path.Combine(GetBasePath(), filePath);
        if (!File.Exists(fullPath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public string GetUrl(string filePath)
    {
        return $"{_config.LocalUrlPrefix}/{filePath}".Replace("\\", "/");
    }

    public Task<bool> DeleteAsync(string filePath)
    {
        var fullPath = Path.Combine(GetBasePath(), filePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> ExistsAsync(string filePath)
    {
        var fullPath = Path.Combine(GetBasePath(), filePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    private string GetBasePath()
    {
        return Path.IsPathRooted(_config.LocalBasePath)
            ? _config.LocalBasePath
            : Path.Combine(AppContext.BaseDirectory, _config.LocalBasePath);
    }
}
