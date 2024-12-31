using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Mqtt.Client.Wrappers;

public class MqttFactoryWrapper : IMqttFactory
{
    private readonly MqttFactory _factory;

    public MqttFactoryWrapper(MqttFactory factory)
    {
        _factory = factory;
    }

    public IManagedMqttClient CreateManagedMqttClient(IMqttNetLogger logger)
    {
        return _factory.CreateManagedMqttClient(logger);
    }
}