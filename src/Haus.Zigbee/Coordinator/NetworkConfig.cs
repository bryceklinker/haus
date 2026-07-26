namespace Haus.Zigbee.Coordinator;

public record NetworkConfig(IeeeAddress MacAddress, ushort PanId, byte Channel);
