using System;
using System.Text;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using MQTTnet;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Tests.Support
{
    public class Zigbee2MqttMessageBuilder
    {
        private const string StateTopicPath = "bridge/state";
        private const string LogTopicPath = "bridge/log";
        private const string InterviewSuccessful = "interview_successful";
        private const string InterviewStarted = "interview_started";
        private const string PairingType = "pairing";
        private readonly string _baseTopicName;
        private string _topicPath;
        private string _state;
        private JObject _root;

        public Zigbee2MqttMessageBuilder(string baseTopicName = "zigbee2mqtt")
        {
            _baseTopicName = baseTopicName;
            _root = new JObject();
        }

        public Zigbee2MqttMessageBuilder WithStateTopic()
        {
            return WithTopicPath(StateTopicPath);
        }
        
        public Zigbee2MqttMessageBuilder WithType(string type)
        {
            _root.Add("type", type);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithInterviewSuccessful()
        {
            return WithMessage(InterviewSuccessful);
        }

        public Zigbee2MqttMessageBuilder WithInterviewStarted()
        {
            return WithMessage(InterviewStarted);
        }

        public Zigbee2MqttMessageBuilder WithPairingType()
        {
            return WithType(PairingType);
        }
        
        public Zigbee2MqttMessageBuilder WithMessage(string message)
        {
            _root.Add("message", message);
            return this;
        }
        
        public Zigbee2MqttMessageBuilder WithTopicPath(string topicPath)
        {
            _topicPath = topicPath;
            return this;
        }

        public Zigbee2MqttMessageBuilder WithDeviceTopic(string friendlyName)
        {
            return WithTopicPath(friendlyName);
        }
        
        public Zigbee2MqttMessageBuilder WithLogTopic()
        {
            return WithTopicPath(LogTopicPath);
        }
        
        public Zigbee2MqttMessageBuilder WithOccupancy(bool value)
        {
            _root.Add("occupancy", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithOccupancyTimeout(int value)
        {
            _root.Add("occupancy_timeout", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithMotionSensitivity(string value)
        {
            _root.Add("motion_sensitivity", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithTemperature(double value)
        {
            _root.Add("temperature", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithBatteryLevel(int value)
        {
            _root.Add("battery", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithIlluminance(int value)
        {
            _root.Add("illuminance", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithIlluminanceLux(int value)
        {
            _root.Add("illuminance_lux", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithLinkQuality(int value)
        {
            _root.Add("linkquality", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithState(string state)
        {
            _state = state;
            return this;
        }
        
        public Zigbee2MqttMessageBuilder WithMeta(Action<Zigbee2MqttMetaBuilder> configureMeta)
        {
            var builder = new Zigbee2MqttMetaBuilder();
            configureMeta(builder);
            _root.Add("meta", builder.Build());
            return this;
        }

        public MqttApplicationMessage BuildMqttMessage()
        {
            try
            {
                var payloadAsString = _topicPath == StateTopicPath ? _state : _root.ToString();
                return new MqttApplicationMessage
                {
                    Topic = $"{_baseTopicName}/{_topicPath}",
                    Payload = Encoding.UTF8.GetBytes(payloadAsString)
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
                return Zigbee2MqttMessage.FromJObject($"{_baseTopicName}/{_topicPath}", _root);
            }
            finally
            {
                _root = new JObject();
            }
        }
    }
}