namespace Haus.Zigbee.Models.Diagnostics;

public record ZigbeeNetworkConfigModel(string MacAddress, ushort PanId, byte Channel);
