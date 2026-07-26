using System.Collections.Generic;

namespace Haus.Zigbee;

public record ZigbeeDevice(IeeeAddress IeeeAddress, ushort NetworkAddress, IReadOnlyList<ZigbeeEndpoint> Endpoints);
