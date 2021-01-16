using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

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
            var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", appName)
                .WriteTo.Console()
                .WriteTo.File(new CompactJsonFormatter(), $"{logsDirectory}/{appName}.log")
                .CreateLogger();
        }
    }
}