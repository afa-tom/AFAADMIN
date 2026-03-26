using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace AFAADMIN.Web.Core.Extensions;

public static class SerilogExtensions
{
    public static IHostBuilder UseAfaSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/afaadmin-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    encoding: System.Text.Encoding.UTF8);
        });
    }
}
