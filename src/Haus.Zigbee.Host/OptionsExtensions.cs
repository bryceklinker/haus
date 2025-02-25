using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host;

public static class OptionsExtensions
{
    public static string GetBaseTopic(this IOptions<ZigbeeOptions> options)
    {
        return options.Value.GetBaseTopic();
    }

    public static string GetEventsTopic(this IOptions<HausOptions> options)
    {
        return options.Value.EventsTopic;
    }
}
