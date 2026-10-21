using System.Collections.Generic;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Host.Storage;

public record KnownZigbeeDeviceRecord(
    string ExternalId,
    ushort NetworkAddress,
    IReadOnlyList<ZigbeeEndpoint> Endpoints,
    bool IsSleepy
);
