using Haus.Zigbee.Host.Configuration;

namespace Haus.Zigbee.Host.Options
{
    public class ZigbeeOptions
    {
        public string DataDirectory { get; set; }
        public string ConfigFile { get; set; }
        
        public Zigbee2MqttConfiguration Config { get; set; }
    }
}