namespace Haus.Zigbee.Zcl;

public record ZclCommand(
    byte TransactionSequenceNumber,
    byte CommandId,
    byte[] Payload,
    bool DisableDefaultResponse,
    ushort? ManufacturerCode = null
)
{
    public ZclFrameHeader ToHeader() =>
        new(
            ZclFrameType.ClusterSpecific,
            ZclDirection.ClientToServer,
            DisableDefaultResponse,
            TransactionSequenceNumber,
            CommandId,
            ManufacturerCode
        );
}

public static class ZclCommandFactory
{
    public static byte[] Encode(ZclCommand command)
    {
        var headerBytes = ZclFrameHeaderCodec.Encode(command.ToHeader());
        return [.. headerBytes, .. command.Payload];
    }
}
