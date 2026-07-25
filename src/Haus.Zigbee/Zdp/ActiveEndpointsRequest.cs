using System;
using System.Collections.Generic;

namespace Haus.Zigbee.Zdp;

public sealed record ActiveEndpointsRequest(byte TransactionSequenceNumber, ushort NetworkAddress);

public sealed record ActiveEndpointsResponse(
    byte TransactionSequenceNumber,
    ZdoStatus Status,
    ushort NetworkAddress,
    IReadOnlyList<byte> EndpointIds
);

public static class ActiveEndpointsResponseCodec
{
    public static ActiveEndpointsResponse Decode(ReadOnlySpan<byte> payload)
    {
        var transactionSequenceNumber = payload[0];
        var status = (ZdoStatus)payload[1];
        if (status != ZdoStatus.Success)
            return new ActiveEndpointsResponse(
                transactionSequenceNumber,
                status,
                NetworkAddress: 0,
                Array.Empty<byte>()
            );

        var networkAddress = (ushort)(payload[2] | (payload[3] << 8));
        var endpointCount = payload[4];
        var endpointIds = payload.Slice(5, endpointCount).ToArray();

        return new ActiveEndpointsResponse(transactionSequenceNumber, status, networkAddress, endpointIds);
    }
}

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
