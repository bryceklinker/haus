using System;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Serial.Frames;

public record ApsDataConfirm(
    byte SequenceNumber,
    byte DeviceState,
    byte RequestId,
    DeconzAddressMode DestinationAddressMode,
    ushort? DestinationShortAddress,
    IeeeAddress? DestinationIeeeAddress,
    byte? DestinationEndpoint,
    byte SourceEndpoint,
    byte ConfirmStatus
);

public record ApsDataConfirmDecoding
{
    private ApsDataConfirmDecoding(bool isSuccessful, ApsDataConfirm? confirm)
    {
        IsSuccessful = isSuccessful;
        Confirm = confirm;
    }

    public bool IsSuccessful { get; }
    public ApsDataConfirm? Confirm { get; }

    public static ApsDataConfirmDecoding Failed { get; } = new(false, null);

    public static ApsDataConfirmDecoding Successful(ApsDataConfirm confirm)
    {
        return new ApsDataConfirmDecoding(true, confirm);
    }
}

public static class ApsDataConfirmCodec
{
    private const int StatusOffset = 2;
    private const byte SuccessStatus = 0x00;
    private const int SequenceNumberOffset = 1;
    private const int DeviceStateOffset = 7;
    private const int RequestIdOffset = 8;
    private const int AddressModeOffset = 9;
    private const int AddressOffset = 10;

    private const int ShortAddressByteLength = 2;
    private const int IeeeAddressByteLength = 8;

    public static ApsDataConfirmDecoding Decode(ReadOnlySpan<byte> frame)
    {
        if (frame[StatusOffset] != SuccessStatus)
            return ApsDataConfirmDecoding.Failed;

        var addressMode = (DeconzAddressMode)frame[AddressModeOffset];
        var destination = DecodeDestination(frame, addressMode);

        return ApsDataConfirmDecoding.Successful(
            new ApsDataConfirm(
                SequenceNumber: frame[SequenceNumberOffset],
                DeviceState: frame[DeviceStateOffset],
                RequestId: frame[RequestIdOffset],
                DestinationAddressMode: addressMode,
                DestinationShortAddress: destination.ShortAddress,
                DestinationIeeeAddress: destination.IeeeAddress,
                DestinationEndpoint: destination.Endpoint,
                SourceEndpoint: frame[destination.SourceEndpointOffset],
                ConfirmStatus: frame[destination.SourceEndpointOffset + 1]
            )
        );
    }

    private static DestinationAddressing DecodeDestination(ReadOnlySpan<byte> frame, DeconzAddressMode mode)
    {
        if (mode == DeconzAddressMode.Ieee)
        {
            var ieeeAddress = new IeeeAddress(ReadUInt64(frame, AddressOffset));
            var afterIeee = AddressOffset + IeeeAddressByteLength;
            return new DestinationAddressing(null, ieeeAddress, frame[afterIeee], afterIeee + 1);
        }

        var shortAddress = ReadUInt16(frame, AddressOffset);
        var afterShort = AddressOffset + ShortAddressByteLength;
        if (mode == DeconzAddressMode.Group)
            return new DestinationAddressing(shortAddress, null, null, afterShort);

        return new DestinationAddressing(shortAddress, null, frame[afterShort], afterShort + 1);
    }

    private readonly record struct DestinationAddressing(
        ushort? ShortAddress,
        IeeeAddress? IeeeAddress,
        byte? Endpoint,
        int SourceEndpointOffset
    );

    private static ushort ReadUInt16(ReadOnlySpan<byte> frame, int offset)
    {
        return (ushort)(frame[offset] | (frame[offset + 1] << 8));
    }

    private static ulong ReadUInt64(ReadOnlySpan<byte> frame, int offset)
    {
        ulong value = 0;
        for (var index = 0; index < IeeeAddressByteLength; index++)
            value |= (ulong)frame[offset + index] << (8 * index);
        return value;
    }
}
