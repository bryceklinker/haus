namespace Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;

public class Zigbee2MqttConfiguration
{
    public MqttConfiguration Mqtt { get; init; } = new();
}

public class MqttConfiguration
{
    public string BaseTopic { get; init; } = "";
    public string Server { get; set; } = "";
}
