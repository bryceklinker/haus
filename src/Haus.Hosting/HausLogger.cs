using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Haus.Hosting;

public interface ILogsDirectoryProvider
{
    string GetLogsDirectory();
}

public class HausLogger : ILogsDirectoryProvider
{
    private const string LogLevelEnvironmentVariableName = "LOG_LEVEL";
    private const LogEventLevel DefaultLogLevel = LogEventLevel.Information;
    private const long LogFileSizeLimit = 1024 * 1024 * 8;
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

    public static void ConfigureDefaultLogging(string appName)
    {
        AppName = appName;
        Log.Logger = CreateDefaultLoggerConfiguration(appName)
            .WriteTo.File(
                new RenderedCompactJsonFormatter(),
                LogFilePath,
                fileSizeLimitBytes: LogFileSizeLimit,
                retainedFileCountLimit: 20
            )
            .CreateLogger();
    }

    public static void ConfigureConsoleOnly(string appName)
    {
        AppName = appName;
        Log.Logger = CreateDefaultLoggerConfiguration(appName).CreateLogger();
    }

    private static LoggerConfiguration CreateDefaultLoggerConfiguration(string appName)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Is(GetLoggingLevel())
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Haus.Mqtt", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", appName)
            .WriteTo.Console();
    }

    private static LogEventLevel GetLoggingLevel()
    {
        return Enum.TryParse(
            Environment.GetEnvironmentVariable(LogLevelEnvironmentVariableName),
            out LogEventLevel level
        )
            ? level
            : DefaultLogLevel;
    }
}
