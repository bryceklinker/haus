using System;
using System.Buffers.Binary;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Zdp;

public record NwkAddrRequest(byte TransactionSequenceNumber, IeeeAddress IeeeAddress);

public record NwkAddrResponse(
    byte TransactionSequenceNumber,
    ZdoStatus Status,
    IeeeAddress IeeeAddress,
    ushort NetworkAddress
);

public static class NwkAddrResponseCodec
{
    private const int TransactionSequenceNumberOffset = 0;
    private const int StatusOffset = 1;
    private const int IeeeAddressOffset = 2;
    private const int NetworkAddressOffset = 10;
    private const int NetworkAddressLength = 2;

    // This response comes straight off the wire, so a truncated payload must produce a null result
    // here rather than throw -- see ActiveEndpointsResponseCodec.Decode for why an exception here
    // would silently stop delivery to every other IndicationReceived subscriber. Trailing bytes past
    // the fixed prefix belong to an Extended Response we never requested, so they are ignored.
    public static NwkAddrResponse? Decode(ReadOnlySpan<byte> payload)
    {
        if (payload.Length <= StatusOffset)
            return null;

        var transactionSequenceNumber = payload[TransactionSequenceNumberOffset];
        var status = (ZdoStatus)payload[StatusOffset];
        if (status != ZdoStatus.Success)
            return new NwkAddrResponse(transactionSequenceNumber, status, default, NetworkAddress: 0);

        if (payload.Length < NetworkAddressOffset + NetworkAddressLength)
            return null;

        var ieeeAddress = new IeeeAddress(BinaryPrimitives.ReadUInt64LittleEndian(payload[IeeeAddressOffset..]));
        var networkAddress = BinaryPrimitives.ReadUInt16LittleEndian(payload[NetworkAddressOffset..]);
        return new NwkAddrResponse(transactionSequenceNumber, status, ieeeAddress, networkAddress);
    }
}

public static class NwkAddrRequestCodec
{
    private const int RequestLength = 11;
    private const int TransactionSequenceNumberOffset = 0;
    private const int IeeeAddressOffset = 1;
    private const int RequestTypeOffset = 9;
    private const int StartIndexOffset = 10;
    private const byte SingleDeviceResponse = 0x00;
    private const byte StartIndex = 0x00;

    public static byte[] Encode(NwkAddrRequest request)
    {
        var bytes = new byte[RequestLength];
        bytes[TransactionSequenceNumberOffset] = request.TransactionSequenceNumber;
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.AsSpan(IeeeAddressOffset), request.IeeeAddress.Value);
        bytes[RequestTypeOffset] = SingleDeviceResponse;
        bytes[StartIndexOffset] = StartIndex;
        return bytes;
    }
}
