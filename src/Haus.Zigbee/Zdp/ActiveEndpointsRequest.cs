using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Haus.Zigbee.Zdp;

public record ActiveEndpointsRequest(byte TransactionSequenceNumber, ushort NetworkAddress);

public record ActiveEndpointsResponse(
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
            return new ActiveEndpointsResponse(transactionSequenceNumber, status, NetworkAddress: 0, []);

        var networkAddress = BinaryPrimitives.ReadUInt16LittleEndian(payload[2..]);
        var endpointCount = payload[4];
        var endpointIds = payload.Slice(5, endpointCount).ToArray();

        return new ActiveEndpointsResponse(transactionSequenceNumber, status, networkAddress, endpointIds);
    }
}

public static class ActiveEndpointsRequestCodec
{
    public static byte[] Encode(ActiveEndpointsRequest request)
    {
        var bytes = new byte[3];
        bytes[0] = request.TransactionSequenceNumber;
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(1), request.NetworkAddress);
        return bytes;
    }
}
