using Haus.Zigbee.Host.Configuration;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host;

public static class OptionsExtensions
{
    public static string GetEventsTopic(this IOptions<HausOptions> options)
    {
        return options.Value.EventsTopic;
    }

    public static string GetZigbeeTopic(this IOptions<HausOptions> options)
    {
        return options.Value.ZigbeeTopic;
    }

    public static string GetZigbeeKnownDeviceSeedTopic(this IOptions<HausOptions> options)
    {
        return options.Value.ZigbeeKnownDeviceSeedTopic;
    }
}
