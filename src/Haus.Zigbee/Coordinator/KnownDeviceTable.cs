using System.Collections.Generic;
using System.Linq;

namespace Haus.Zigbee.Coordinator;

public sealed class KnownDeviceTable
{
    public IReadOnlyList<ZigbeeDevice> GetDevices()
    {
        return Enumerable.Empty<ZigbeeDevice>().ToList();
    }
}
