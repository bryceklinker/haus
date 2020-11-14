using System.Text;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Models
{
    public class Zigbee2MqttMessage
    {
        private readonly JObject _root;
        private Zigbee2MqttMeta _meta;
        public string Topic { get; }
        public string Json => _root.ToString();
        public string Type => SafeGetValue<string>("type");
        public string Message => SafeGetValue<string>("message");
        public bool HasMeta => Meta != null;

        public Zigbee2MqttMeta Meta
        {
            get
            {
                if (_meta != null)
                    return _meta;
                
                if (_root.TryGetValue("meta", out _))
                {
                    return (_meta = new Zigbee2MqttMeta(_root.GetValue("meta")));
                }

                return null;
            }
        }
        public int? Battery => SafeGetValue<int?>("battery");
        public int? Illuminance => SafeGetValue<int?>("illuminance");
        public int? IlluminanceLux => SafeGetValue<int?>("illuminance_lux");
        public int? LinkQuality => SafeGetValue<int?>("linkquality");
        public string MotionSensitivity => SafeGetValue<string>("motion_sensitivity");
        public bool? Occupancy => SafeGetValue<bool?>("occupancy");
        public int? OccupancyTimeout => SafeGetValue<int?>("occupancy_timeout");
        public double? Temperature => SafeGetValue<double?>("temperature");

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

        public string GetFriendlyNameFromTopic()
        {
            return Topic.Split('/')[1];
        }

        private T SafeGetValue<T>(string propertyName)
        {
            return _root.TryGetValue(propertyName, out var token) 
                ? token.ToObject<T>() 
                : default;
        }
    }
}