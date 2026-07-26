using System.Collections.Generic;

namespace Haus.Zigbee;

public record ZigbeeDeviceJoined(
    IeeeAddress IeeeAddress,
    ushort NetworkAddress,
    IReadOnlyList<ZigbeeEndpoint> Endpoints,
    string ManufacturerName,
    string ModelIdentifier
);
