namespace Haus.Core.Models.Zigbee;

public record ZigbeeNetworkConfigModel(string MacAddress, ushort PanId, byte Channel);
