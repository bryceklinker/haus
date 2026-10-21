using System;
using Haus.Core.Models.Devices.Resolvers;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Health;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausZigbee(
        this IServiceCollection services,
        IConfiguration config,
        Action<DbContextOptionsBuilder>? configureDatabase = null
    )
    {
        services.AddHealthChecks().AddHausMqttHealthChecks();

        return services
            .AddSingleton<IDeviceTypeResolver>(sp => new DeviceTypeResolver(
                sp.GetRequiredService<IOptions<HausOptions>>().Value.DeviceTypeOptions
            ))
            // A singleton-safe factory, rather than a scoped DbContext, so DeviceAddressRegistry
            // (itself a singleton -- its in-memory cache must outlive any single request/scope)
            // can create a short-lived context per operation without becoming a captive dependency.
            // The provider itself is injected by the caller (Program.cs picks Sqlite; tests pick
            // EF's InMemory provider), the same provider-injection-from-host pattern Haus.Core uses.
            .AddDbContextFactory<HausZigbeeDbContext>(
                configureDatabase
                    ?? (
                        opts =>
                            opts.UseSqlite(
                                config["Database:ConnectionString"],
                                sqlite => sqlite.MigrationsAssembly(GetMigrationsAssemblyName())
                            )
                    )
            )
            .AddSingleton<IKnownZigbeeDeviceStore, KnownZigbeeDeviceStore>()
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

    private static string GetMigrationsAssemblyName()
    {
        var name = typeof(HausZigbeeDbContext).Assembly.GetName().Name;
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name;
    }
}
