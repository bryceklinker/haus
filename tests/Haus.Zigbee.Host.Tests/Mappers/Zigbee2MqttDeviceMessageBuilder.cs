using System.Text;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using MQTTnet;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Tests.Mappers
{
    public class Zigbee2MqttDeviceMessageBuilder
    {
        private string _friendlyName;
        private string _baseTopicName;
        private JObject _root;

        public Zigbee2MqttDeviceMessageBuilder(string baseTopicName = "zigbee2mqtt")
        {
            _baseTopicName = baseTopicName;
            _root = new JObject();
        }

        public Zigbee2MqttDeviceMessageBuilder WithBaseTopicName(string baseTopic)
        {
            _baseTopicName = baseTopic;
            return this;
        }
        
        public Zigbee2MqttDeviceMessageBuilder WithFriendlyName(string friendlyName)
        {
            _friendlyName = friendlyName;
            return this;
        }
        
        public Zigbee2MqttDeviceMessageBuilder WithOccupancy(bool value)
        {
            _root.Add("occupancy", value);
            return this;
        }

        public Zigbee2MqttDeviceMessageBuilder WithOccupancyTimeout(int value)
        {
            _root.Add("occupancy_timeout", value);
            return this;
        }

        public Zigbee2MqttDeviceMessageBuilder WithMotionSensitivity(string value)
        {
            _root.Add("motion_sensitivity", value);
            return this;
        }

        public Zigbee2MqttDeviceMessageBuilder WithTemperature(double value)
        {
            _root.Add("temperature", value);
            return this;
        }

        public Zigbee2MqttDeviceMessageBuilder WithBatteryLevel(int value)
        {
            _root.Add("battery", value);
            return this;
        }

        public Zigbee2MqttDeviceMessageBuilder WithIlluminance(int value)
        {
            _root.Add("illuminance", value);
            return this;
        }

        public Zigbee2MqttDeviceMessageBuilder WithIlluminanceLux(int value)
        {
            _root.Add("illuminance_lux", value);
            return this;
        }

        public Zigbee2MqttDeviceMessageBuilder WithLinkQuality(int value)
        {
            _root.Add("linkquality", value);
            return this;
        }

        public MqttApplicationMessage BuildMqttMessage()
        {
            try
            {
                return new MqttApplicationMessage
                {
                    Topic = $"{_baseTopicName}/{_friendlyName}",
                    Payload = Encoding.UTF8.GetBytes(_root.ToString())
                };
            }
            finally
            {
                _root = new JObject();
            }
        }

        public Zigbee2MqttMessage BuildZigbee2MqttMessage()
        {
            try
            {
                return new Zigbee2MqttMessage($"{_baseTopicName}/{_friendlyName}", _root);
            }
            finally
            {
                _root = new JObject();
            }
        }
    }
}