using Haus.Core.Models.Common;

namespace Haus.Mqtt.Client.Settings;

public class HausMqttSettings
{
    public string Server { get; set; }
    public string EventsTopic { get; init; } = DefaultHausMqttTopics.EventsTopic;
    public string CommandsTopic { get; init; } = DefaultHausMqttTopics.CommandsTopic;
    public string UnknownTopic { get; init; } = DefaultHausMqttTopics.UnknownTopic;
    public string HealthTopic { get; init; } = DefaultHausMqttTopics.HealthTopic;
}
