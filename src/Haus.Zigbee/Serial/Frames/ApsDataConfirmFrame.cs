using System;
using System.Buffers.Binary;
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

    // This frame comes straight off the wire from a real device, so a short or malformed frame must
    // report failure here rather than throwing an IndexOutOfRangeException -- see ZclFrameHeaderCodec.Decode
    // for why an exception here would silently stop delivery to every other IndicationReceived subscriber.
    public static ApsDataConfirmDecoding Decode(ReadOnlySpan<byte> frame)
    {
        if (frame.Length <= StatusOffset)
            return ApsDataConfirmDecoding.Failed;
        if (frame[StatusOffset] != SuccessStatus)
            return ApsDataConfirmDecoding.Failed;
        if (frame.Length <= AddressModeOffset)
            return ApsDataConfirmDecoding.Failed;

        var addressMode = (DeconzAddressMode)frame[AddressModeOffset];
        var destination = TryDecodeDestination(frame, addressMode);
        if (destination is null)
            return ApsDataConfirmDecoding.Failed;

        if (frame.Length <= destination.Value.SourceEndpointOffset + 1)
            return ApsDataConfirmDecoding.Failed;

        return ApsDataConfirmDecoding.Successful(
            new ApsDataConfirm(
                SequenceNumber: frame[SequenceNumberOffset],
                DeviceState: frame[DeviceStateOffset],
                RequestId: frame[RequestIdOffset],
                DestinationAddressMode: addressMode,
                DestinationShortAddress: destination.Value.ShortAddress,
                DestinationIeeeAddress: destination.Value.IeeeAddress,
                DestinationEndpoint: destination.Value.Endpoint,
                SourceEndpoint: frame[destination.Value.SourceEndpointOffset],
                ConfirmStatus: frame[destination.Value.SourceEndpointOffset + 1]
            )
        );
    }

    private static DestinationAddressing? TryDecodeDestination(ReadOnlySpan<byte> frame, DeconzAddressMode mode)
    {
        if (mode == DeconzAddressMode.Ieee)
        {
            var afterIeee = AddressOffset + IeeeAddressByteLength;
            if (frame.Length <= afterIeee)
                return null;

            var ieeeAddress = new IeeeAddress(BinaryPrimitives.ReadUInt64LittleEndian(frame[AddressOffset..]));
            return new DestinationAddressing(null, ieeeAddress, frame[afterIeee], afterIeee + 1);
        }

        var afterShort = AddressOffset + ShortAddressByteLength;
        if (frame.Length < afterShort)
            return null;

        var shortAddress = BinaryPrimitives.ReadUInt16LittleEndian(frame[AddressOffset..]);
        if (mode == DeconzAddressMode.Group)
            return new DestinationAddressing(shortAddress, null, null, afterShort);

        if (frame.Length <= afterShort)
            return null;

        return new DestinationAddressing(shortAddress, null, frame[afterShort], afterShort + 1);
    }

    private readonly record struct DestinationAddressing(
        ushort? ShortAddress,
        IeeeAddress? IeeeAddress,
        byte? Endpoint,
        int SourceEndpointOffset
    );
}
