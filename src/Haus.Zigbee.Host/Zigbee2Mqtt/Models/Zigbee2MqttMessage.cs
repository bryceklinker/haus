using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Models;

public class Zigbee2MqttMessage(string topic, string? raw = null, JToken? root = null)
{
    private Zigbee2MqttMeta? _meta;
    public string Topic { get; } = topic;
    public string? Raw { get; } = raw;

    public JObject? PayloadObject => root as JObject;
    public JArray? PayloadArray => root as JArray;

    public bool IsJson => root != null;
    public string? Json => Raw;
    public string Type => SafeGetValue<string>("type");
    public string Message => SafeGetValue<string>("message");
    public bool HasMeta => Meta != null;

    public Zigbee2MqttMeta? Meta => GetMetaData();
    public int? Battery => SafeGetValue<int?>("battery");
    public int? Illuminance => SafeGetValue<int?>("illuminance");
    public int? IlluminanceLux => SafeGetValue<int?>("illuminance_lux");
    public int? LinkQuality => SafeGetValue<int?>("linkquality");
    public string MotionSensitivity => SafeGetValue<string>("motion_sensitivity");
    public bool? Occupancy => SafeGetValue<bool?>("occupancy");
    public int? OccupancyTimeout => SafeGetValue<int?>("occupancy_timeout");
    public double? Temperature => SafeGetValue<double?>("temperature");

    public static Zigbee2MqttMessage FromJToken(string topic, JToken jToken)
    {
        return new Zigbee2MqttMessage(topic, jToken.ToString(), jToken);
    }

    public string GetFriendlyNameFromTopic()
    {
        return Topic.Split('/')[1];
    }

    private T? SafeGetValue<T>(string propertyName)
    {
        if (PayloadObject == null)
            return default;

        return PayloadObject.TryGetValue(propertyName, out var token) ? token.ToObject<T>() : default;
    }

    private Zigbee2MqttMeta? GetMetaData()
    {
        if (!IsJson)
            return null;

        if (_meta != null)
            return _meta;

        return PayloadObject.TryGetValue("meta", out _)
            ? _meta = new Zigbee2MqttMeta(JObject.Parse(PayloadObject.GetValue("meta").ToString()))
            : null;
    }
}
