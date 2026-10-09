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

    // Only updates an already-known device -- re-querying the Node Descriptor for a device this
    // table has never heard of has nothing to refresh, unlike UpdateNetworkAddress (which can
    // legitimately learn about a brand-new device from a resolve broadcast). A lost race against a
    // concurrent write is an acceptable no-op here: this is a best-effort refresh, not a path that
    // must never lose data the way UpdateNetworkAddress's endpoint preservation is.
    public void UpdateIsSleepy(IeeeAddress ieeeAddress, bool isSleepy)
    {
        if (_devices.TryGetValue(ieeeAddress, out var existing))
            _devices.TryUpdate(ieeeAddress, existing with { IsSleepy = isSleepy }, existing);
    }

    public IReadOnlyList<ZigbeeDevice> GetDevices()
    {
        return _devices.Values.ToList();
    }

    public bool TryGet(IeeeAddress address, [MaybeNullWhen(false)] out ZigbeeDevice device)
    {
        return _devices.TryGetValue(address, out device);
    }

    // Linear scan is acceptable here: this mirrors GetDevices' own O(n) materialization, and the
    // known-device table is bounded by how many physical devices a single coordinator manages.
    public bool TryGetByNetworkAddress(ushort networkAddress, [MaybeNullWhen(false)] out ZigbeeDevice device)
    {
        device = _devices.Values.FirstOrDefault(d => d.NetworkAddress == networkAddress);
        return device is not null;
    }
}
