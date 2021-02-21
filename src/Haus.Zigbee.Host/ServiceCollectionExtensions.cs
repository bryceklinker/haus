using System;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Health;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Health;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mqtt;
using Haus.Zigbee.Host.Zigbee2Mqtt.Node;
using Haus.Zigbee.Host.Zigbee2Mqtt.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Zigbee.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausZigbee(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddHealthChecks()
                .AddHausMqttHealthChecks()
                .AddCheck<Zigbee2MqttHealthCheck>("Zigbee2Mqtt");
            
            return services
                .AddSingleton<IZigbeeMqttClientFactory, ZigbeeMqttClientFactory>()
                .AddTransient<IMqttMessageMapper, MqttMessageMapper>()
                .AddTransient<IDeviceTypeResolver, DeviceTypeResolver>()
                .AddHausToZigbeeMappers()
                .AddZigbeeToHausMappers()
                .AddTransient<IZigbee2MqttConfigurationWriter, Zigbee2MqttConfigurationWriter>()
                .Configure<ZigbeeOptions>(config.GetSection("ZigBee"))
                .Configure<HausOptions>(config.GetSection("Haus"))
                .Configure<HausMqttSettings>(config.GetSection("Haus"))
                .AddHausMqtt()
                .AddSingleton<INodeZigbeeProcess, NodeZigbeeProcess>()
                .AddSingleton<IHealthCheckPublisher, ZigbeeHostHealthPublisher>()
                .AddHostedService<NodeZigbeeBackgroundService>()
                .AddHostedService<ZigbeeToHausRelay>()
                .Configure<HealthCheckPublisherOptions>(opts =>
                {
                    opts.Period = TimeSpan.FromSeconds(10);
                });
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