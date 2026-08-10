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
    private const int TransactionSequenceNumberOffset = 0;
    private const int StatusOffset = 1;
    private const int NetworkAddressOffset = 2;
    private const int EndpointCountOffset = 4;
    private const int EndpointIdsOffset = 5;

    // This response comes straight off the wire, so a truncated payload must produce a null
    // result here rather than throw -- see ZclFrameHeaderCodec.Decode for why an exception here
    // would silently stop delivery to every other IndicationReceived subscriber.
    public static ActiveEndpointsResponse? Decode(ReadOnlySpan<byte> payload)
    {
        if (payload.Length <= StatusOffset)
            return null;

        var transactionSequenceNumber = payload[TransactionSequenceNumberOffset];
        var status = (ZdoStatus)payload[StatusOffset];
        if (status != ZdoStatus.Success)
            return new ActiveEndpointsResponse(transactionSequenceNumber, status, NetworkAddress: 0, []);

        if (payload.Length <= EndpointCountOffset)
            return null;

        var networkAddress = BinaryPrimitives.ReadUInt16LittleEndian(payload[NetworkAddressOffset..]);
        var endpointCount = payload[EndpointCountOffset];
        if (payload.Length < EndpointIdsOffset + endpointCount)
            return null;

        var endpointIds = payload.Slice(EndpointIdsOffset, endpointCount).ToArray();
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
