using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Models;

public record ZigbeeCommandRequest(
    ApsDestination Destination,
    byte SourceEndpoint,
    ushort ProfileId,
    ushort ClusterId,
    byte CommandId,
    byte[] Payload,
    bool DisableDefaultResponse,
    bool RequestApsAck = false
);
