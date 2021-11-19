namespace Haus.Zigbee.Host.Zigbee2Mqtt.Configuration
{
    public class Zigbee2MqttConfiguration
    {
        public MqttConfiguration Mqtt { get; set; }
    }

    public class MqttConfiguration
    {
        public string BaseTopic { get; set; }
        public string Server { get; set; }
    }
}