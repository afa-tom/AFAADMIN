using Lazy.Captcha.Core;
using Lazy.Captcha.Core.Generator;
using Lazy.Captcha.Core.Generator.Image.Option;
using Microsoft.Extensions.DependencyInjection;

namespace AFAADMIN.Tools;

public static class ToolsServiceCollectionExtensions
{
    /// <summary>
    /// 注册工具链服务
    /// </summary>
    public static IServiceCollection AddAfaTools(this IServiceCollection services)
    {
        // 验证码
        services.AddCaptcha(options =>
        {
            options.CaptchaType = CaptchaType.ARITHMETIC;
            options.CodeLength = 2;
            options.ExpirySeconds = 120;
            options.ImageOption = new CaptchaImageGeneratorOption
            {
                Width = 150,
                Height = 50,
                FontSize = 28
            };
        });

        return services;
    }
}
