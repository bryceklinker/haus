using Serilog;

namespace Haus.Site.Host.Shared.Logging;

public static class HausSiteHostLogConfigurator
{
    public static LoggerConfiguration CreateLogConfig()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.WithProperty("app", "haus_site")
            .WriteTo.BrowserConsole();
    }

    public static ILogger CreateLogger()
    {
        return CreateLogConfig().CreateLogger();
    }
}
