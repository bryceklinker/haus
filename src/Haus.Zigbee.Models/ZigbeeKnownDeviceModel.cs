using System.Collections.Generic;

namespace Haus.Zigbee.Models.Diagnostics;

public record ZigbeeKnownDeviceModel(
    string IeeeAddress,
    ushort NetworkAddress,
    IReadOnlyList<ZigbeeEndpointModel> Endpoints,
    bool IsSleepy
);
