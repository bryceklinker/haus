using System;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Health;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
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
        services.AddHealthChecks().AddHausMqttHealthChecks();

        return services
            .AddTransient<IDeviceTypeResolver, DeviceTypeResolver>()
            .AddSingleton<DeviceAddressRegistry>()
            .AddTransient<DevicesMapper>()
            .AddTransient<DeviceJoinedMapper>()
            .AddTransient<DeviceEventMapper>()
            .AddTransient<HausDiscoveryToZigbeeMapper>()
            .AddTransient<HausLightingToZigbeeMapper>()
            .AddTransient<ZigbeeInboundRelay>()
            .AddTransient<ZigbeeOutboundRelay>()
            .AddTransient<DeviceBackfillService>()
            .Configure<ZigbeeConnectionOptions>(config.GetSection("Zigbee"))
            .AddHausZigbee()
            .Configure<HausOptions>(config.GetSection("Haus"))
            .Configure<HausMqttSettings>(config.GetSection("Haus"))
            .AddHausMqtt()
            .AddSingleton<IHealthCheckPublisher, ZigbeeHostHealthPublisher>()
            .AddHostedService<ZigbeeHausBridge>()
            .Configure<HealthCheckPublisherOptions>(opts =>
            {
                opts.Period = TimeSpan.FromSeconds(10);
            });
    }
}
