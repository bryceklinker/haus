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
    private const int SequenceNumberOffset = 1;
    private const int DeviceStateOffset = 7;
    private const int RequestIdOffset = 8;
    private const int AddressModeOffset = 9;
    private const int AddressOffset = 10;

    public static ApsDataConfirmDecoding Decode(ReadOnlySpan<byte> frame)
    {
        var addressMode = (DeconzAddressMode)frame[AddressModeOffset];
        var destinationShortAddress = (ushort)(frame[AddressOffset] | (frame[AddressOffset + 1] << 8));
        var destinationEndpoint = frame[AddressOffset + 2];
        var sourceEndpoint = frame[AddressOffset + 3];
        var confirmStatus = frame[AddressOffset + 4];

        return ApsDataConfirmDecoding.Successful(
            new ApsDataConfirm(
                SequenceNumber: frame[SequenceNumberOffset],
                DeviceState: frame[DeviceStateOffset],
                RequestId: frame[RequestIdOffset],
                DestinationAddressMode: addressMode,
                DestinationShortAddress: destinationShortAddress,
                DestinationIeeeAddress: null,
                DestinationEndpoint: destinationEndpoint,
                SourceEndpoint: sourceEndpoint,
                ConfirmStatus: confirmStatus
            )
        );
    }
}
