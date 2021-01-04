using Haus.Hosting;
using Haus.Utilities.Common.Cli;
using Haus.Utilities.TypeScript.GenerateModels;
using Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Utilities
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausUtilities(this IServiceCollection services)
        {
            return services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly)
                .AddHttpClient()
                .AddHausLogging()
                .AddTransient<ITypeScriptModelGenerator, TypeScriptModelGenerator>()
                .AddTransient<IDeviceTypeOptionsHtmlParser, DeviceTypeOptionsHtmlParser>()
                .AddTransient<IDeviceTypeOptionsMerger, DeviceTypeOptionsMerger>()
                .AddSingleton<ICommandFactory, CommandFactory>();
        }
    }
}