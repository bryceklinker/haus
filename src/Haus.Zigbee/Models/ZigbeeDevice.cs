using System.Collections.Generic;

namespace Haus.Zigbee.Models;

public record ZigbeeDevice(IeeeAddress IeeeAddress, ushort NetworkAddress, IReadOnlyList<ZigbeeEndpoint> Endpoints);
