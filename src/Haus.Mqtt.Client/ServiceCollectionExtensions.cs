using Haus.Mqtt.Client.Logging;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Diagnostics;

namespace Haus.Mqtt.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausMqtt(this IServiceCollection services)
        {
            return services.AddTransient<IMqttFactory, MqttFactory>()
                .AddTransient<IMqttNetLogger, MqttLogger>()
                .AddSingleton<IHausMqttClientFactory, HausMqttClientFactory>();
        }
    }
}