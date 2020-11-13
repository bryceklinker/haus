
using System.Text;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Models
{
    public class Zigbee2MqttPayload
    {
        public string Topic { get; }
        private readonly JObject _root;
        private Zigbee2MqttMeta _meta;
        
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
        public Zigbee2MqttPayload(string topic, byte[] bytes)
        {
            Topic = topic;
            _root = JObject.Parse(Encoding.UTF8.GetString(bytes));
        }
    }
}