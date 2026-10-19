namespace Haus.Zigbee.Host.Storage;

public record UpsertKnownZigbeeDeviceArgs(
    string ExternalId,
    ushort NetworkAddress,
    string EndpointsJson,
    bool IsSleepy
);
