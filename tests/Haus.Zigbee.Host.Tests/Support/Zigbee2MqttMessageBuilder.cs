using System;
using System.Text;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using MQTTnet;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Tests.Support;

public class Zigbee2MqttMessageBuilder(string baseTopicName = Defaults.ZigbeeOptions.BaseTopic)
{
    private const string StateTopicPath = "bridge/state";
    private const string LogTopicPath = "bridge/log";
    private const string ConfigPath = "bridge/config";
    private static readonly string DevicesTopicPath = $"{ConfigPath}/devices";
    private const string InterviewSuccessful = "interview_successful";
    private const string PairingType = "pairing";
    private string _topicPath;
    private string _state;
    private JObject _payloadObject = new();
    private JArray _payloadArray = new();

    public Zigbee2MqttMessageBuilder WithStateTopic()
    {
        return WithTopicPath(StateTopicPath);
    }

    public Zigbee2MqttMessageBuilder WithType(string type)
    {
        _payloadObject.Add("type", type);
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
        _payloadObject.Add("message", message);
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
        _payloadObject.Add("occupancy", value);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithOccupancyTimeout(int value)
    {
        _payloadObject.Add("occupancy_timeout", value);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithMotionSensitivity(string value)
    {
        _payloadObject.Add("motion_sensitivity", value);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithTemperature(double value)
    {
        _payloadObject.Add("temperature", value);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithBatteryLevel(int value)
    {
        _payloadObject.Add("battery", value);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithIlluminance(int value)
    {
        _payloadObject.Add("illuminance", value);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithIlluminanceLux(int value)
    {
        _payloadObject.Add("illuminance_lux", value);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithLinkQuality(int value)
    {
        _payloadObject.Add("linkquality", value);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithState(string state)
    {
        _state = state;
        return this;
    }

    public Zigbee2MqttMessageBuilder WithDeviceInPayload(Action<JObject> configureDevice = null)
    {
        var device = new JObject();
        configureDevice?.Invoke(device);
        _payloadArray.Add(device);
        return this;
    }

    public Zigbee2MqttMessageBuilder WithMeta(Action<Zigbee2MqttMetaBuilder> configureMeta)
    {
        var builder = new Zigbee2MqttMetaBuilder();
        configureMeta(builder);
        _payloadObject.Add("meta", builder.BuildJToken());
        return this;
    }

    public Zigbee2MqttMessageBuilder WithDevicesTopic()
    {
        return WithTopicPath(DevicesTopicPath);
    }

    public MqttApplicationMessage BuildMqttMessage()
    {
        try
        {
            var payloadAsString = GetRootMessageAsJson();
            return new MqttApplicationMessage
            {
                Topic = $"{baseTopicName}/{_topicPath}",
                PayloadSegment = Encoding.UTF8.GetBytes(payloadAsString),
            };
        }
        finally
        {
            _payloadObject = new JObject();
            _payloadArray = new JArray();
        }
    }

    public Zigbee2MqttMessage BuildZigbee2MqttMessage()
    {
        try
        {
            return Zigbee2MqttMessage.FromJToken($"{baseTopicName}/{_topicPath}", JToken.Parse(GetRootMessageAsJson()));
        }
        finally
        {
            _payloadObject = new JObject();
            _payloadArray = new JArray();
        }
    }

    private string GetRootMessageAsJson()
    {
        if (_topicPath == StateTopicPath)
            return _state;

        if (_topicPath == DevicesTopicPath)
            return _payloadArray.ToString();

        return _payloadObject.ToString();
    }
}
