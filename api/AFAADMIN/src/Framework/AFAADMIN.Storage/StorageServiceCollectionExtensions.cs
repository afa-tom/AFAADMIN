using AFAADMIN.Common.Config;
using AFAADMIN.Storage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.Storage;

public static class StorageServiceCollectionExtensions
{
    /// <summary>
    /// 注册文件存储服务（根据配置自动切换 Local / MinIO）
    /// </summary>
    public static IServiceCollection AddAfaStorage(this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = new StorageConfig();
        configuration.GetSection("Storage").Bind(config);
        services.AddSingleton(config);

        services.AddSingleton<IStorageProvider>(sp =>
        {
            var logger1 = sp.GetRequiredService<ILogger<LocalStorageProvider>>();
            var logger2 = sp.GetRequiredService<ILogger<MinioStorageProvider>>();

            return config.Provider.ToLower() switch
            {
                "minio" => new MinioStorageProvider(config, logger2),
                _ => new LocalStorageProvider(config, logger1)
            };
        });

        return services;
    }
}
