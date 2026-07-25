namespace Haus.Zigbee.Coordinator;

public sealed record NetworkConfig(IeeeAddress MacAddress, ushort PanId, byte Channel);
