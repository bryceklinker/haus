using System.Collections.Generic;

namespace Haus.Zigbee;

public sealed record ZigbeeDevice(
    IeeeAddress IeeeAddress,
    ushort NetworkAddress,
    IReadOnlyList<ZigbeeEndpoint> Endpoints
);
