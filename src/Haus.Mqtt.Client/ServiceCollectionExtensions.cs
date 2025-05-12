using Haus.Mqtt.Client.Health;
using Haus.Mqtt.Client.Logging;
using Haus.Mqtt.Client.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Diagnostics;

namespace Haus.Mqtt.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausMqtt(this IServiceCollection services)
    {
        return services
            .AddTransient<MqttFactory>()
            .AddTransient<IMqttFactory, MqttFactoryWrapper>()
            .AddTransient<IMqttNetLogger, MqttLogger>()
            .AddSingleton<IHausMqttClientFactory, HausMqttClientFactory>();
    }

    public static IHealthChecksBuilder AddHausMqttHealthChecks(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<HausMqttHealthCHeck>("haus-mqtt");
    }
}
