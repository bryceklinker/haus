using YamlDotNet.Serialization;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Configuration
{
    public class Zigbee2MqttConfiguration
    {
        [YamlMember(Alias = "homeassistant")]
        public bool HomeAssistant { get; set; }
        
        [YamlMember(Alias = "permit_join")]
        public bool PermitJoin { get; set; }
        
        [YamlMember(Alias = "mqtt")]
        public MqttConfiguration Mqtt { get; set; }
        
        [YamlMember(Alias = "serial")]
        public SerialConfiguration Serial { get; set; }
    }

    public class MqttConfiguration
    {
        [YamlMember(Alias = "base_topic")]
        public string BaseTopic { get; set; }
        [YamlMember(Alias = "server")]
        public string Server { get; set; }
    }

    public class SerialConfiguration
    {
        [YamlMember(Alias = "adapter")]
        public string Adapter { get; set; }
    }
}