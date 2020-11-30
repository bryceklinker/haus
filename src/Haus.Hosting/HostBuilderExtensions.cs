using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Haus.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseHausLogging(this IHostBuilder builder, string appName)
        {
            HausLogger.Configure(appName);
            return builder.UseSerilog();
        }
    }
}