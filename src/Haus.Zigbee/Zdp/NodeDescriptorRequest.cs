namespace Haus.Zigbee.Zdp;

public sealed record NodeDescriptorRequest(byte TransactionSequenceNumber, ushort NetworkAddress);

public static class NodeDescriptorRequestCodec
{
    public static byte[] Encode(NodeDescriptorRequest request)
    {
        return new[]
        {
            request.TransactionSequenceNumber,
            (byte)(request.NetworkAddress & 0xff),
            (byte)(request.NetworkAddress >> 8),
        };
    }
}
