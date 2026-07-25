using System;

namespace Haus.Zigbee.Zcl;

public sealed record ZclCommand(
    byte TransactionSequenceNumber,
    byte CommandId,
    byte[] Payload,
    bool DisableDefaultResponse,
    ushort? ManufacturerCode = null
);

public static class ZclCommandBuilder
{
    public static byte[] Build(ZclCommand command)
    {
        var header = new ZclFrameHeader(
            ZclFrameType.ClusterSpecific,
            ZclDirection.ClientToServer,
            command.DisableDefaultResponse,
            command.TransactionSequenceNumber,
            command.CommandId,
            command.ManufacturerCode
        );

        var headerBytes = ZclFrameHeaderCodec.Encode(header);
        var frame = new byte[headerBytes.Length + command.Payload.Length];
        Array.Copy(headerBytes, frame, headerBytes.Length);
        Array.Copy(command.Payload, 0, frame, headerBytes.Length, command.Payload.Length);
        return frame;
    }
}
