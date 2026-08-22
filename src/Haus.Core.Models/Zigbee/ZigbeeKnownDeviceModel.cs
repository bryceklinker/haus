using System;
using System.Collections.Generic;

namespace Haus.Core.Models.Zigbee;

public record ZigbeeKnownDeviceModel(
    string IeeeAddress,
    ushort? NetworkAddress,
    string? ManufacturerName,
    string? ModelIdentifier,
    IReadOnlyList<ZigbeeEndpointModel> Endpoints,
    DateTimeOffset LastSeenAt
);
