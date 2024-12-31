using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Mqtt.Client.Wrappers;

public interface IMqttFactory
{
    IManagedMqttClient CreateManagedMqttClient(IMqttNetLogger logger);
}