namespace Haus.Zigbee.Host.Zigbee.Configuration;

public class ZigbeeOptions
{
    public Zigbee2MqttConfiguration Config { get; init; } = new();

    public string GetBaseTopic()
    {
        return Config.Mqtt.BaseTopic;
    }
}
