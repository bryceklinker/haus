using System;
using System.Text;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;

public interface IZigbee2MqttMessageFactory
{
    Zigbee2MqttMessage Create(MqttApplicationMessage message);
}

public class Zigbee2MqttMessageFactory : IZigbee2MqttMessageFactory
{
    private readonly ILogger<Zigbee2MqttMessageFactory> _logger;

    public Zigbee2MqttMessageFactory(ILogger<Zigbee2MqttMessageFactory> logger)
    {
        _logger = logger;
    }

    public Zigbee2MqttMessage Create(MqttApplicationMessage message)
    {
        var payloadAsString = message.PayloadSegment != null ? Encoding.UTF8.GetString(message.PayloadSegment) : null;
        return new Zigbee2MqttMessage(message.Topic, payloadAsString, CreateJObjectFromPayload(payloadAsString));
    }

    private JToken CreateJObjectFromPayload(string payload)
    {
        try
        {
            return JToken.Parse(payload);
        }
        catch (Exception)
        {
            _logger.LogInformation("Failed to turn payload {0} into JObject", payload);
            return null;
        }
    }
}