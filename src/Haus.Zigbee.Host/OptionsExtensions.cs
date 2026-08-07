using Haus.Zigbee.Host.Configuration;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host;

public static class OptionsExtensions
{
    public static string GetEventsTopic(this IOptions<HausOptions> options)
    {
        return options.Value.EventsTopic;
    }
}
