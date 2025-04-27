using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Haus.Acceptance.Tests.Support.Zigbee2Mqtt;

public record Zigbee2MqttMessage
{
    [JsonIgnore]
    public string Topic { get; init; } = Zigbee2MqttTopics.Base;

    public string? Type { get; init; }

    public string? Message { get; init; }

    public Dictionary<string, string?>? Meta { get; init; }
}
