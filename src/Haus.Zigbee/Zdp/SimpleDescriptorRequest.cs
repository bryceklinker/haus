namespace Haus.Zigbee.Zdp;

public sealed record SimpleDescriptorRequest(byte TransactionSequenceNumber, ushort NetworkAddress, byte Endpoint);

public static class SimpleDescriptorCodec
{
    public static byte[] EncodeRequest(SimpleDescriptorRequest request)
    {
        return new[]
        {
            request.TransactionSequenceNumber,
            (byte)(request.NetworkAddress & 0xff),
            (byte)(request.NetworkAddress >> 8),
            request.Endpoint,
        };
    }
}
