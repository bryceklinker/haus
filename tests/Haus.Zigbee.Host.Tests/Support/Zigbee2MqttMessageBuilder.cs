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
        private const string ConfigPath = "bridge/config";
        private static readonly string GetDevicesPath = $"{ConfigPath}/devices";
        private const string InterviewSuccessful = "interview_successful";
        private const string PairingType = "pairing";
        private readonly string _baseTopicName;
        private string _topicPath;
        private string _state;
        private JObject _rootObject;
        private JArray _rootArray;

        public Zigbee2MqttMessageBuilder(string baseTopicName = Defaults.ZigbeeOptions.BaseTopic)
        {
            _baseTopicName = baseTopicName;
            _rootObject = new JObject();
            _rootArray = new JArray();
        }

        public Zigbee2MqttMessageBuilder WithStateTopic()
        {
            return WithTopicPath(StateTopicPath);
        }
        
        public Zigbee2MqttMessageBuilder WithType(string type)
        {
            _rootObject.Add("type", type);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithInterviewSuccessful()
        {
            return WithMessage(InterviewSuccessful);
        }

        public Zigbee2MqttMessageBuilder WithPairingType()
        {
            return WithType(PairingType);
        }
        
        public Zigbee2MqttMessageBuilder WithMessage(string message)
        {
            _rootObject.Add("message", message);
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
            _rootObject.Add("occupancy", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithOccupancyTimeout(int value)
        {
            _rootObject.Add("occupancy_timeout", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithMotionSensitivity(string value)
        {
            _rootObject.Add("motion_sensitivity", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithTemperature(double value)
        {
            _rootObject.Add("temperature", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithBatteryLevel(int value)
        {
            _rootObject.Add("battery", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithIlluminance(int value)
        {
            _rootObject.Add("illuminance", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithIlluminanceLux(int value)
        {
            _rootObject.Add("illuminance_lux", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithLinkQuality(int value)
        {
            _rootObject.Add("linkquality", value);
            return this;
        }

        public Zigbee2MqttMessageBuilder WithState(string state)
        {
            _state = state;
            return this;
        }

        public Zigbee2MqttMessageBuilder WithDevice(Action<JObject> configureDevice = null)
        {
            var device = new JObject();
            configureDevice?.Invoke(device);
            _rootArray.Add(device);
            return this;
        }
        
        public Zigbee2MqttMessageBuilder WithMeta(Action<Zigbee2MqttMetaBuilder> configureMeta)
        {
            var builder = new Zigbee2MqttMetaBuilder();
            configureMeta(builder);
            _rootObject.Add("meta", builder.BuildJToken());
            return this;
        }

        public Zigbee2MqttMessageBuilder WithGetDevicesTopic()
        {
            return WithTopicPath($"{ConfigPath}/devices");
        }

        public MqttApplicationMessage BuildMqttMessage()
        {
            try
            {
                var payloadAsString = GetRootMessageAsJson();
                return new MqttApplicationMessage
                {
                    Topic = $"{_baseTopicName}/{_topicPath}",
                    Payload = Encoding.UTF8.GetBytes(payloadAsString)
                };
            }
            finally
            {
                _rootObject = new JObject();
            }
        }

        public Zigbee2MqttMessage BuildZigbee2MqttMessage()
        {
            try
            {
                return Zigbee2MqttMessage.FromJToken($"{_baseTopicName}/{_topicPath}", JToken.Parse(GetRootMessageAsJson()));
            }
            finally
            {
                _rootObject = new JObject();
            }
        }

        private string GetRootMessageAsJson()
        {
            if (_topicPath == StateTopicPath)
                return _state;

            if (_topicPath == GetDevicesPath)
                return _rootArray.ToString();

            return _rootObject.ToString();
        }
    }
}