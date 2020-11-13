
using System.Text;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Models
{
    public class Zigbee2MqttMessage
    {
        public string Topic { get; }
        private readonly JObject _root;
        private Zigbee2MqttMeta _meta;

        public string Json => _root.ToString();
        public string Type => _root.Value<string>("type");
        public string Message => _root.Value<string>("message");
        public bool HasMeta => Meta != null;
        public Zigbee2MqttMeta Meta
        {
            get
            {
                if (_meta != null)
                    return _meta;
                
                return _root.TryGetValue("meta", out var token) 
                    ? (_meta = new Zigbee2MqttMeta(token)) 
                    : null;
            }
        }
        
        public Zigbee2MqttMessage(string topic, byte[] bytes)
            : this(topic, Encoding.Default.GetString(bytes))
        {
        }

        public Zigbee2MqttMessage(string topic, string payload)
            : this(topic, JObject.Parse(payload))
        {
        }
        
        public Zigbee2MqttMessage(string topic, JObject root)
        {
            _root = root;
            Topic = topic;
        }
    }
}