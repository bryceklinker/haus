using Haus.Zigbee;

namespace Haus.Zigbee.Host.Zigbee;

public static class ExternalIdMap
{
    public static string ToExternalId(IeeeAddress address)
    {
        return address.ToString();
    }

    public static bool TryParseAddress(string? externalId, out IeeeAddress address)
    {
        return IeeeAddress.TryParse(externalId, out address);
    }
}
