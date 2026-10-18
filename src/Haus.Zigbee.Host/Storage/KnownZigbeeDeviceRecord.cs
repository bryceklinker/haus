namespace Haus.Zigbee.Host.Storage;

public record KnownZigbeeDeviceRecord(string ExternalId, ushort NetworkAddress, string EndpointsJson, bool IsSleepy);
