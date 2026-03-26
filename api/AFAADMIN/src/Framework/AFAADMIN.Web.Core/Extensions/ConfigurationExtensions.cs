using Microsoft.Extensions.Configuration;

namespace AFAADMIN.Web.Core.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// 加载 configs 目录下的分离配置文件
    /// </summary>
    public static IConfigurationBuilder AddAfaConfigurations(this IConfigurationBuilder builder,
        string basePath)
    {
        var configDir = Path.Combine(basePath, "configs");

        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }

        // 扫描 configs 目录下所有 json 文件
        var configFiles = Directory.GetFiles(configDir, "*.json");
        foreach (var file in configFiles)
        {
            builder.AddJsonFile(file, optional: true, reloadOnChange: true);
        }

        return builder;
    }
}
