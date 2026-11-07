namespace Haus.Core.Models.Common;

public static class DefaultHausMqttTopics
{
    public const string CommandsTopic = "haus/commands";
    public const string EventsTopic = "haus/events";
    public const string UnknownTopic = "haus/idk";
    public const string HealthTopic = "haus/health";
    public const string ZigbeeTopic = "haus/zigbee";

    // Dedicated (not the shared ZigbeeTopic firehose) so this retained snapshot can't be clobbered
    // by an unrelated non-retained message landing on the same topic.
    public const string ZigbeeKnownDeviceSeedTopic = "haus/zigbee/known_device_seed";
}
