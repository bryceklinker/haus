namespace Haus.Zigbee.Host.Zigbee2Mqtt.Configuration
{
    public class ZigbeeOptions
    {
        public bool OverwriteConfig { get; set; } = true;
        public string DataDirectory { get; set; }
        public string ConfigFile { get; set; }
        public Zigbee2MqttConfiguration Config { get; set; }

        public string GetBaseTopic()
        {
            return Config.Mqtt.BaseTopic;
        }
    }
}