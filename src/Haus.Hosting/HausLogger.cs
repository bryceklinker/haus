using System;
using Serilog;
using Serilog.Events;

namespace Haus.Hosting
{
    public static class HausLogger
    {
        public static void Fatal(Exception exception, string message)
        {
            Log.Fatal(exception, message);
        }
        
        public static void EnsureFlushed()
        {
            Log.CloseAndFlush();
        }
        
        public static void Configure(string appName)
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