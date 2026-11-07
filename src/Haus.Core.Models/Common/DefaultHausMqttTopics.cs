namespace Haus.Core.Models.Common;

public static class DefaultHausMqttTopics
{
    public const string CommandsTopic = "haus/commands";
    public const string EventsTopic = "haus/events";
    public const string UnknownTopic = "haus/idk";
    public const string HealthTopic = "haus/health";
    public const string ZigbeeTopic = "haus/zigbee";

    // Dedicated topic: a shared firehose would let a non-retained message clobber this retained one.
    public const string ZigbeeKnownDeviceSeedTopic = "haus/zigbee/known_device_seed";
}
