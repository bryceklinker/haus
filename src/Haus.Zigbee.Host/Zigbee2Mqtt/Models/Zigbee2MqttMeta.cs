using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Models
{
    public class Zigbee2MqttMeta
    {
        private readonly JToken _root;

        public string Description => _root.Value<string>("description");
        public string FriendlyName => _root.Value<string>("friendly_name");
        public string Model => _root.Value<string>("model");
        public bool Supported => _root.Value<bool>("supported");
        public string Vendor => _root.Value<string>("vendor");
        
        public Zigbee2MqttMeta(JToken root)
        {
            _root = root;
        }
    }
}