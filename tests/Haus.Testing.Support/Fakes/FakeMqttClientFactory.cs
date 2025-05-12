using Haus.Mqtt.Client.Wrappers;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Testing.Support.Fakes;

public class FakeMqttClientFactory : IMqttFactory
{
    public FakeMqttClient Client { get; } = new();

    public IManagedMqttClient CreateManagedMqttClient(IMqttNetLogger logger)
    {
        return Client;
    }
}
