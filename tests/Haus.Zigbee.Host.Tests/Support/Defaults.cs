using Haus.Core.Models.Common;

namespace Haus.Zigbee.Host.Tests.Support
{
    public static class Defaults
    {
        public static class HausOptions
        {
            public const string EventsTopic = DefaultHausMqttTopics.EventsTopic;
            public const string CommandsTopic = DefaultHausMqttTopics.CommandsTopic;
            public const string UnknownTopic = DefaultHausMqttTopics.UnknownTopic;
            public const string HealthTopic = DefaultHausMqttTopics.HealthTopic;
        }

        public static class ZigbeeOptions
        {
            public const string BaseTopic = "zigbee2mqtt";
        }
    }
}