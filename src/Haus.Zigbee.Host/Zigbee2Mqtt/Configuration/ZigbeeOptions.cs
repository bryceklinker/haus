namespace Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;

public class ZigbeeOptions
{
    public Zigbee2MqttConfiguration Config { get; set; }

    public string GetBaseTopic()
    {
        return Config.Mqtt.BaseTopic;
    }
}