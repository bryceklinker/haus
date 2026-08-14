using Haus.Hosting;
using Haus.Utilities.Common.Cli;
using Haus.Utilities.Packaging;
using Haus.Utilities.TypeScript.GenerateModels;
using Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Utilities;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausUtilities(this IServiceCollection services)
    {
        return services
            .AddHttpClient()
            .AddHausLogging()
            .AddTransient<ITypeScriptModelGenerator, TypeScriptModelGenerator>()
            .AddTransient<IDeviceTypeOptionsParser, DeviceTypeOptionsParser>()
            .AddTransient<IDeviceTypeOptionsMerger, DeviceTypeOptionsMerger>()
            .AddTransient<IDebComposeVersionPinner, DebComposeVersionPinner>()
            .AddSingleton<ICommandFactory, CommandFactory>();
    }
}
