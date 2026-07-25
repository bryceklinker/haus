namespace Haus.Zigbee.Zcl;

public enum ZclFrameType : byte
{
    Global = 0b00,
    ClusterSpecific = 0b01,
}

public enum ZclDirection : byte
{
    ClientToServer = 0,
    ServerToClient = 1,
}

public sealed record ZclFrameHeader(
    ZclFrameType FrameType,
    ZclDirection Direction,
    bool DisableDefaultResponse,
    byte TransactionSequenceNumber,
    byte CommandId,
    ushort? ManufacturerCode = null
);

public static class ZclFrameHeaderCodec
{
    public static byte[] Encode(ZclFrameHeader header)
    {
        return new byte[] { 0x00, header.TransactionSequenceNumber, header.CommandId };
    }
}
