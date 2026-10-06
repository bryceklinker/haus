using System;
using Haus.Core.Models.Devices.Resolvers;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Health;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausZigbee(this IServiceCollection services, IConfiguration config)
    {
        services.AddHealthChecks().AddHausMqttHealthChecks();

        return services
            .AddSingleton<IDeviceTypeResolver>(sp => new DeviceTypeResolver(
                sp.GetRequiredService<IOptions<HausOptions>>().Value.DeviceTypeOptions
            ))
            .AddSingleton<DeviceAddressRegistry>()
            .AddSingleton<DevicesMapper>()
            .AddSingleton<DeviceJoinedMapper>()
            .AddSingleton<DeviceEventMapper>()
            .AddSingleton<HausDiscoveryToZigbeeMapper>()
            .AddSingleton<HausLightingToZigbeeMapper>()
            .AddSingleton<ZigbeeInboundRelay>()
            .AddSingleton<ZigbeeOutboundRelay>()
            .AddSingleton<ZigbeeDiagnosticsPublisher>()
            .AddSingleton<DeviceBackfillService>()
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
