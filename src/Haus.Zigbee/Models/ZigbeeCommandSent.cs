using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Models;

public record ZigbeeCommandSent(ApsDestination Destination, ushort ClusterId, byte CommandId);
