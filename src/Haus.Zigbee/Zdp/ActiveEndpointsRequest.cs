namespace Haus.Zigbee.Zdp;

public sealed record ActiveEndpointsRequest(byte TransactionSequenceNumber, ushort NetworkAddress);

public static class ActiveEndpointsRequestCodec
{
    public static byte[] Encode(ActiveEndpointsRequest request)
    {
        return
        [
            request.TransactionSequenceNumber,
            (byte)(request.NetworkAddress & 0xff),
            (byte)(request.NetworkAddress >> 8),
        ];
    }
}
