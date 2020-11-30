using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Factories;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Node;
using Haus.Zigbee.Host.Zigbee2Mqtt.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;

namespace Haus.Zigbee.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausZigbee(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddSingleton<IMqttFactory>(p => new MqttFactory())
                .AddTransient<IMapperFactory, MapperFactory>()
                .AddTransient<IZigbeeToHausModelMapper, ZigbeeToHausModelMapper>()
                .AddTransient<IZigbee2MqttMessageFactory, Zigbee2MqttMessageFactory>()
                .AddTransient<IZigbee2MqttConfigurationWriter, Zigbee2MqttConfigurationWriter>()
                .Configure<ZigbeeOptions>(config.GetSection("ZigBee"))
                .Configure<HausOptions>(config.GetSection("Haus"))
                .AddSingleton<INodeZigbeeProcess, NodeZigbeeProcess>()
                .AddHostedService<NodeZigbeeBackgroundService>()
                .AddHostedService<ZigbeeToHausRelay>();
        }
    }
}