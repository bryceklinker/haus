using System;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Haus.Logging
{
    public static class HausLogging
    {
        public static void Configure(string application)
        {
            Log.Logger =  new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", application)
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.Console()
                .CreateLogger();
        }

        public static void LogFatal(Exception ex, string template)
        {
            Log.Fatal(ex, template);
        }

        public static void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }

        public static IHostBuilder UseHuasLogging(this IHostBuilder builder)
        {
            return builder.UseSerilog();
        }
    }
}