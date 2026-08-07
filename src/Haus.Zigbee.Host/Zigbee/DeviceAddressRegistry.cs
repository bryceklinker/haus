using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Haus.Zigbee.Host.Zigbee;

public class DeviceAddressRegistry
{
    private readonly ConcurrentDictionary<ushort, string> _externalIdsByNetworkAddress = new();

    public void Register(ushort networkAddress, string externalId)
    {
        _externalIdsByNetworkAddress[networkAddress] = externalId;
    }

    public bool TryGetExternalId(ushort networkAddress, [MaybeNullWhen(false)] out string externalId)
    {
        return _externalIdsByNetworkAddress.TryGetValue(networkAddress, out externalId);
    }
}
