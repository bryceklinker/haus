namespace Haus.Zigbee.Zcl;

public record ZclCommand(
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
        var headerBytes = ZclFrameHeaderCodec.Encode(ToHeader(command));
        return [.. headerBytes, .. command.Payload];
    }

    private static ZclFrameHeader ToHeader(ZclCommand command) =>
        new(
            ZclFrameType.ClusterSpecific,
            ZclDirection.ClientToServer,
            command.DisableDefaultResponse,
            command.TransactionSequenceNumber,
            command.CommandId,
            command.ManufacturerCode
        );
}
