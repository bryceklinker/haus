using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Node;
using Haus.Zigbee.Host.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Zigbee.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausZigbee(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddTransient<IZigbee2MqttConfigurationWriter, Zigbee2MqttConfigurationWriter>()
                .Configure<ZigbeeOptions>(config.GetSection("ZigBee"))
                .AddSingleton<INodeZigbeeProcess, NodeZigbeeProcess>()
                .AddHostedService<ZigBeeWorker>();
        }
    }
}