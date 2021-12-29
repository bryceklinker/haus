using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haus.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseHausLogging(this IHostBuilder builder, string appName)
    {
        HausLogger.ConfigureDefaultLogging(appName);
        return builder.UseSerilog();
    }
}