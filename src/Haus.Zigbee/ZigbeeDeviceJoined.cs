using System.Collections.Generic;

namespace Haus.Zigbee;

public sealed record ZigbeeDeviceJoined(
    IeeeAddress IeeeAddress,
    ushort NetworkAddress,
    IReadOnlyList<ZigbeeEndpoint> Endpoints,
    string ManufacturerName,
    string ModelIdentifier
);
