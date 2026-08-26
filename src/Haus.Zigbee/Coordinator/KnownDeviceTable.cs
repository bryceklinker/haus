using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Coordinator;

public class KnownDeviceTable
{
    private readonly ConcurrentDictionary<IeeeAddress, ZigbeeDevice> _devices = new();

    public void AddOrUpdate(ZigbeeDevice device)
    {
        _devices[device.IeeeAddress] = device;
    }

    // A separate read-then-write (TryGet an existing entry's Endpoints, then AddOrUpdate a rebuilt
    // ZigbeeDevice) is not atomic against a concurrent full AddOrUpdate -- e.g. an on-demand address
    // resolution racing a device's own re-announce could silently lose the announce's freshly
    // discovered endpoints. ConcurrentDictionary.AddOrUpdate's factories are retried against the
    // latest value on a concurrent write, so this can only ever apply on top of whatever the most
    // recent entry actually is.
    public void UpdateNetworkAddress(IeeeAddress ieeeAddress, ushort networkAddress)
    {
        _devices.AddOrUpdate(
            ieeeAddress,
            addValueFactory: _ => new ZigbeeDevice(ieeeAddress, networkAddress, []),
            updateValueFactory: (_, existing) => existing with { NetworkAddress = networkAddress }
        );
    }

    public IReadOnlyList<ZigbeeDevice> GetDevices()
    {
        return _devices.Values.ToList();
    }

    public bool TryGet(IeeeAddress address, [MaybeNullWhen(false)] out ZigbeeDevice device)
    {
        return _devices.TryGetValue(address, out device);
    }
}
