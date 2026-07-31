using System.Collections.Concurrent;
using System.Collections.Generic;
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

    public IReadOnlyList<ZigbeeDevice> GetDevices()
    {
        return _devices.Values.ToList();
    }

    public bool TryGet(IeeeAddress address, out ZigbeeDevice device)
    {
        return _devices.TryGetValue(address, out device!);
    }
}
