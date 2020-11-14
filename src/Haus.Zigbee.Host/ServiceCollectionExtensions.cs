using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Node;
using Haus.Zigbee.Host.Zigbee2Mqtt.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Zigbee.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausZigbee(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddTransient<IMapperFactory, MapperFactory>()
                .AddTransient<IZigbeeToHausModelMapper, ZigbeeToHausModelMapper>()
                .AddTransient<IZigbee2MqttConfigurationWriter, Zigbee2MqttConfigurationWriter>()
                .Configure<ZigbeeOptions>(config.GetSection("ZigBee"))
                .Configure<HausOptions>(config.GetSection("Haus"))
                .AddSingleton<INodeZigbeeProcess, NodeZigbeeProcess>()
                .AddHostedService<NodeZigbeeBackgroundService>();
        }
    }
}