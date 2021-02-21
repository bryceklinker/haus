using Haus.Core.Models.Common;

namespace Haus.Mqtt.Client.Settings
{
    public class HausMqttSettings
    {
        public string Server { get; set; }
        public string EventsTopic { get; set; } = DefaultHausMqttTopics.EventsTopic;
        public string CommandsTopic { get; set; } = DefaultHausMqttTopics.CommandsTopic;
        public string UnknownTopic { get; set; } = DefaultHausMqttTopics.UnknownTopic;
        public string HealthTopic { get; set; } = DefaultHausMqttTopics.HealthTopic;
    }
}