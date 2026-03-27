namespace AFAADMIN.Common.Config;

/// <summary>
/// 文件存储配置
/// </summary>
public class StorageConfig
{
    /// <summary>
    /// 存储类型: Local / Minio
    /// </summary>
    public string Provider { get; set; } = "Local";

    /// <summary>
    /// 本地存储根路径
    /// </summary>
    public string LocalBasePath { get; set; } = "uploads";

    /// <summary>
    /// 本地存储访问URL前缀
    /// </summary>
    public string LocalUrlPrefix { get; set; } = "/files";

    /// <summary>
    /// MinIO 配置
    /// </summary>
    public MinioConfig Minio { get; set; } = new();
}

public class MinioConfig
{
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "afaadmin";
    public bool UseSSL { get; set; } = false;
}
