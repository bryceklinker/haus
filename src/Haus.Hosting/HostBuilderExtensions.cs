using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Haus.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseHausLogging(this IHostBuilder builder, string appName)
        {
            ConfigureSerilogLogger(appName);
            return builder.UseSerilog();
        }

        private static void ConfigureSerilogLogger(string appName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", appName)
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}