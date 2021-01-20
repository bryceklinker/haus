using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Haus.Hosting
{
    public interface ILogsDirectoryProvider
    {
        string GetLogsDirectory();
    }

    public class HausLogger : ILogsDirectoryProvider
    {
        private const long LogFileSizeLimit = 1024 * 1024 *  8;
        private static readonly string LogsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "haus-logs");
        private static string AppName { get; set; }
        private static string LogFilePath => Path.Combine(LogsDirectory, $"{AppName}.log");

        public string GetLogsDirectory()
        {
            return LogsDirectory;
        }

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
            AppName = appName;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", appName)
                .WriteTo.Console()
                .WriteTo.File(new RenderedCompactJsonFormatter(), LogFilePath, fileSizeLimitBytes: LogFileSizeLimit, retainedFileCountLimit: 20)
                .CreateLogger();
        }
    }
}