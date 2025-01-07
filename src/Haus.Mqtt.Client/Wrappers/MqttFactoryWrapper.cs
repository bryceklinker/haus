using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Mqtt.Client.Wrappers;

public class MqttFactoryWrapper(MqttFactory factory) : IMqttFactory
{
    public IManagedMqttClient CreateManagedMqttClient(IMqttNetLogger logger)
    {
        return factory.CreateManagedMqttClient(logger);
    }
}