using System;

namespace Haus.Zigbee.Serial.Frames;

public enum DeconzAddressMode : byte
{
    Group = 0x01,
    Nwk = 0x02,
    Ieee = 0x03,
}

public sealed record ApsDataConfirm(
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

public sealed record ApsDataConfirmDecoding
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
        var isIeee = addressMode == DeconzAddressMode.Ieee;
        var destinationShortAddress = isIeee ? (ushort?)null : ReadUInt16(frame, AddressOffset);
        var destinationIeeeAddress = isIeee ? (IeeeAddress?)new IeeeAddress(ReadUInt64(frame, AddressOffset)) : null;
        var addressByteLength = isIeee ? IeeeAddressByteLength : ShortAddressByteLength;

        var hasDestinationEndpoint = addressMode != DeconzAddressMode.Group;
        var afterAddress = AddressOffset + addressByteLength;
        var destinationEndpoint = hasDestinationEndpoint ? (byte?)frame[afterAddress] : null;
        var sourceEndpointOffset = hasDestinationEndpoint ? afterAddress + 1 : afterAddress;

        return ApsDataConfirmDecoding.Successful(
            new ApsDataConfirm(
                SequenceNumber: frame[SequenceNumberOffset],
                DeviceState: frame[DeviceStateOffset],
                RequestId: frame[RequestIdOffset],
                DestinationAddressMode: addressMode,
                DestinationShortAddress: destinationShortAddress,
                DestinationIeeeAddress: destinationIeeeAddress,
                DestinationEndpoint: destinationEndpoint,
                SourceEndpoint: frame[sourceEndpointOffset],
                ConfirmStatus: frame[sourceEndpointOffset + 1]
            )
        );
    }

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
