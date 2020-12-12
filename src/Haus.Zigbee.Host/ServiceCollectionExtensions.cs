using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
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
                .AddTransient<IMqttMessageMapper, MqttMessageMapper>()
                .AddTransient<IDeviceTypeResolver, DeviceTypeResolver>()
                .AddHausToZigbeeMappers()
                .AddZigbeeToHausMappers()
                .AddTransient<IZigbee2MqttConfigurationWriter, Zigbee2MqttConfigurationWriter>()
                .Configure<ZigbeeOptions>(config.GetSection("ZigBee"))
                .Configure<HausOptions>(config.GetSection("Haus"))
                .AddSingleton<INodeZigbeeProcess, NodeZigbeeProcess>()
                .AddHostedService<NodeZigbeeBackgroundService>()
                .AddHostedService<ZigbeeToHausRelay>();
        }

        private static IServiceCollection AddHausToZigbeeMappers(this IServiceCollection services)
        {
            return services
                
                .AddTransient<IToZigbeeMapper, HausDiscoveryToZigbeeMapper>()
                .AddTransient<IToZigbeeMapper, HausLightingToZigbeeMapper>()
                .AddTransient<IHausToZigbeeMapper, HausToZigbeeMapper>();
        }

        private static IServiceCollection AddZigbeeToHausMappers(this IServiceCollection services)
        {
            return services
                .AddTransient<IToHausMapper, DevicesMapper>()
                .AddTransient<IToHausMapper, DeviceEventMapper>()
                .AddTransient<IToHausMapper, InterviewSuccessfulMapper>()
                .AddTransient<IUnknownMessageMapper, UnknownMessageMapper>()
                .AddTransient<IZigbeeToHausMapper, ZigbeeToHausMapper>()
                .AddTransient<IZigbee2MqttMessageFactory, Zigbee2MqttMessageFactory>();
        }
    }
}