using System.Collections.Generic;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Host.Storage;

public record RegisterKnownDevice(
    string ExternalId,
    ushort NetworkAddress,
    IReadOnlyList<ZigbeeEndpoint> Endpoints,
    bool IsSleepy
);
