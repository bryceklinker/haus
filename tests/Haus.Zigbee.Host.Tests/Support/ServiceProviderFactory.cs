using System;
using Haus.Mqtt.Client.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Haus.Zigbee.Host.Tests.Support;

public static class ServiceProviderFactory
{
    public static IServiceProvider Create(
        IConfiguration? configuration = null,
        IMqttFactory? mqttFactory = null,
        IZigbeeCoordinator? zigbeeCoordinator = null,
        Action<IServiceCollection>? configureServices = null
    )
    {
        // Every test instance gets its own isolated in-memory database instead of touching a real
        // Sqlite file on disk -- the production connection string is irrelevant here.
        var services = new ServiceCollection()
            .AddLogging(builder => builder.ClearProviders())
            .AddHausZigbee(
                configuration ?? ConfigurationFactory.CreateConfig(),
                opts => opts.UseInMemoryDatabase(Guid.NewGuid().ToString())
            );

        if (mqttFactory != null)
        {
            services.RemoveAll<IMqttFactory>();
            services.AddSingleton(mqttFactory);
        }

        if (zigbeeCoordinator != null)
        {
            services.RemoveAll<IZigbeeCoordinator>();
            services.AddSingleton(zigbeeCoordinator);
        }

        configureServices?.Invoke(services);

        return services.BuildServiceProvider();
    }
}
