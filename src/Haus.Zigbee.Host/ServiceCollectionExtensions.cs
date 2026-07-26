using System;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Health;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Configuration;
using Haus.Zigbee.Host.Zigbee.Health;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.Factories;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Zigbee.Host;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausZigbee(this IServiceCollection services, IConfiguration config)
    {
        services.AddHealthChecks().AddHausMqttHealthChecks().AddCheck<Zigbee2MqttHealthCheck>("Zigbee2Mqtt");

        return services
            .AddTransient<IDeviceTypeResolver, DeviceTypeResolver>()
            .AddHausToZigbeeMappers()
            .AddZigbeeToHausMappers()
            .AddTransient<ZigbeeInboundRelay>()
            .AddTransient<ZigbeeOutboundRelay>()
            .Configure<ZigbeeOptions>(config.GetSection("ZigBee"))
            .Configure<ZigbeeConnectionOptions>(config.GetSection("ZigBee:Serial"))
            .AddHausZigbee()
            .Configure<HausOptions>(config.GetSection("Haus"))
            .Configure<HausMqttSettings>(config.GetSection("Haus"))
            .AddHausMqtt()
            .AddSingleton<IHealthCheckPublisher, ZigbeeHostHealthPublisher>()
            .AddHostedService<ZigbeeToHausRelay>()
            .Configure<HealthCheckPublisherOptions>(opts =>
            {
                opts.Period = TimeSpan.FromSeconds(10);
            });
    }

    private static IServiceCollection AddHausToZigbeeMappers(this IServiceCollection services)
    {
        return services
            .AddTransient<HausDiscoveryToZigbeeMapper>()
            .AddTransient<HausLightingToZigbeeMapper>()
            .AddTransient<IHausToZigbeeMapper, HausToZigbeeMapper>();
    }

    private static IServiceCollection AddZigbeeToHausMappers(this IServiceCollection services)
    {
        return services
            .AddSingleton<DeviceAddressRegistry>()
            .AddTransient<DevicesMapper>()
            .AddTransient<DeviceJoinedMapper>()
            .AddTransient<DeviceEventMapper>()
            .AddTransient<IUnknownMessageMapper, UnknownMessageMapper>()
            .AddTransient<IZigbeeToHausMapper, ZigbeeToHausMapper>()
            .AddTransient<IZigbee2MqttMessageFactory, Zigbee2MqttMessageFactory>();
    }
}
