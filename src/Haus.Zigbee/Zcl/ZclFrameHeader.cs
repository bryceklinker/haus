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
        var frameControl = BuildFrameControl(header);
        return new byte[] { frameControl, header.TransactionSequenceNumber, header.CommandId };
    }

    private static byte BuildFrameControl(ZclFrameHeader header)
    {
        var frameControl = (int)header.FrameType;
        frameControl |= (int)header.Direction << 3;
        frameControl |= header.DisableDefaultResponse ? 1 << 4 : 0;
        return (byte)frameControl;
    }
}
