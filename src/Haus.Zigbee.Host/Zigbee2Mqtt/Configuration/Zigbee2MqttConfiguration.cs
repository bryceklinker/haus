using YamlDotNet.Serialization;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Configuration
{
    public class Zigbee2MqttConfiguration
    {
        [YamlMember(Alias = "mqtt")]
        public MqttConfiguration Mqtt { get; set; }
    }

    public class MqttConfiguration
    {
        [YamlMember(Alias = "base_topic")]
        public string BaseTopic { get; set; }
        [YamlMember(Alias = "server")]
        public string Server { get; set; }
    }
}