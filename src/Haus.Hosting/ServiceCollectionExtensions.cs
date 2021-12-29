using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Haus.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausLogging(this IServiceCollection services)
    {
        return services
            .AddSingleton<ILogsDirectoryProvider, HausLogger>()
            .AddLogging(builder => builder.AddSerilog());
    }
}