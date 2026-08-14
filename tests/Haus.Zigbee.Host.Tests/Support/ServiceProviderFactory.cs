using System;
using Haus.Mqtt.Client.Wrappers;
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
        var services = new ServiceCollection()
            .AddLogging(builder => builder.ClearProviders())
            .AddHausZigbee(configuration ?? ConfigurationFactory.CreateConfig());

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
