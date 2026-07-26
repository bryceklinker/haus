using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee;

public sealed record ZigbeeCommandRequest(
    ApsDestination Destination,
    byte SourceEndpoint,
    ushort ProfileId,
    ushort ClusterId,
    byte CommandId,
    byte[] Payload,
    bool DisableDefaultResponse
);
