using System.Collections.Generic;
using System.Linq;

namespace Haus.Acceptance.Tests.Support.Zigbee2Mqtt;

public class Zigbee2MqttMessageBuilder
{
    private Zigbee2MqttMessage _message = new();

    public Zigbee2MqttMessageBuilder WithTopic(string topic)
    {
        _message = _message with { Topic = topic };
        return this;
    }

    public Zigbee2MqttMessageBuilder WithLogTopic()
    {
        return WithTopic(Zigbee2MqttTopics.Log);
    }

    public Zigbee2MqttMessageBuilder WithType(string type)
    {
        _message = _message with { Type = type };
        return this;
    }

    public Zigbee2MqttMessageBuilder WithMessage(string message)
    {
        _message = _message with { Message = message };
        return this;
    }

    public Zigbee2MqttMessageBuilder WithMeta(Dictionary<string, string?> meta)
    {
        var currentMeta = _message.Meta ?? new Dictionary<string, string?>();
        _message = _message with { Meta = currentMeta.UnionBy(meta, k => k.Key).ToDictionary() };
        return this;
    }

    public Zigbee2MqttMessageBuilder WithPairing()
    {
        return WithType("pairing");
    }

    public Zigbee2MqttMessageBuilder WithInterviewSuccessful()
    {
        return WithMessage("interview_successful");
    }

    public Zigbee2MqttMessageBuilder WithPhillipsLightMeta(string friendlyName)
    {
        return WithMeta(
            new Dictionary<string, string?>
            {
                { "friendly_name", friendlyName },
                { "vendor", "Philips" },
                { "model", "929002335001" },
            }
        );
    }

    public Zigbee2MqttMessageBuilder WithPhillipsMotionSensorMeta(string friendlyName)
    {
        return WithMeta(
            new Dictionary<string, string?>
            {
                { "friendly_name", friendlyName },
                { "vendor", "Philips" },
                { "model", "9290012607" },
            }
        );
    }

    public Zigbee2MqttMessage Build()
    {
        try
        {
            return _message;
        }
        finally
        {
            _message = new Zigbee2MqttMessage();
        }
    }
}
