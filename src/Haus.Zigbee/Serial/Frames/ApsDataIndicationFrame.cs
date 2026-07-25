using System;
using Haus.Zigbee;

namespace Haus.Zigbee.Serial.Frames;

public enum DeconzAddressMode : byte
{
    Group = 0x01,
    Nwk = 0x02,
    Ieee = 0x03,
    NwkAndIeee = 0x04,
}

public sealed record ApsDataIndicationFrame(
    byte DeviceState,
    DeconzAddressMode DestinationAddressMode,
    ushort? DestinationNwkAddress,
    IeeeAddress? DestinationIeeeAddress,
    byte DestinationEndpoint,
    DeconzAddressMode SourceAddressMode,
    ushort SourceNwkAddress,
    IeeeAddress? SourceIeeeAddress,
    byte SourceEndpoint,
    ushort ProfileId,
    ushort ClusterId,
    byte[] AsduPayload,
    byte LinkQualityIndicator,
    sbyte? Rssi
);

public static class ApsDataIndicationFrameCodec
{
    private const int StatusOffset = 2;
    private const int DeviceStateOffset = 7;
    private const int DestinationAddressModeOffset = 8;
    private const int AddressingOffset = 9;
    private const byte SuccessStatus = 0x00;

    public static ApsDataIndicationFrame? Decode(ReadOnlySpan<byte> frame)
    {
        if (frame[StatusOffset] != SuccessStatus)
            return null;

        var deviceState = frame[DeviceStateOffset];
        var destinationAddressMode = (DeconzAddressMode)frame[DestinationAddressModeOffset];

        var offset = AddressingOffset;
        var destinationNwkAddress = default(ushort?);
        var destinationIeeeAddress = default(IeeeAddress?);
        if (destinationAddressMode == DeconzAddressMode.Ieee)
        {
            destinationIeeeAddress = ReadIeeeAddress(frame, offset);
            offset += 8;
        }
        else
        {
            destinationNwkAddress = ReadUInt16(frame, offset);
            offset += 2;
        }

        var destinationEndpoint = frame[offset];
        offset += 1;

        var sourceAddressMode = (DeconzAddressMode)frame[offset];
        offset += 1;
        if (sourceAddressMode != DeconzAddressMode.Nwk && sourceAddressMode != DeconzAddressMode.NwkAndIeee)
            return null;

        var sourceNwkAddress = ReadUInt16(frame, offset);
        offset += 2;

        var sourceIeeeAddress = default(IeeeAddress?);
        if (sourceAddressMode == DeconzAddressMode.NwkAndIeee)
        {
            sourceIeeeAddress = ReadIeeeAddress(frame, offset);
            offset += 8;
        }

        var sourceEndpoint = frame[offset];
        offset += 1;

        var profileId = ReadUInt16(frame, offset);
        offset += 2;

        var clusterId = ReadUInt16(frame, offset);
        offset += 2;

        var asduLength = ReadUInt16(frame, offset);
        offset += 2;

        var asduPayload = frame.Slice(offset, asduLength).ToArray();
        offset += asduLength;

        offset += 2; // reserved / protocol-version-dependent bytes

        var linkQualityIndicator = frame[offset];
        offset += 1;

        var rssi = offset < frame.Length ? (sbyte?)(sbyte)frame[offset] : null;

        return new ApsDataIndicationFrame(
            deviceState,
            destinationAddressMode,
            destinationNwkAddress,
            destinationIeeeAddress,
            destinationEndpoint,
            sourceAddressMode,
            sourceNwkAddress,
            sourceIeeeAddress,
            sourceEndpoint,
            profileId,
            clusterId,
            asduPayload,
            linkQualityIndicator,
            rssi
        );
    }

    private static ushort ReadUInt16(ReadOnlySpan<byte> frame, int offset)
    {
        return (ushort)(frame[offset] | (frame[offset + 1] << 8));
    }

    private static IeeeAddress ReadIeeeAddress(ReadOnlySpan<byte> frame, int offset)
    {
        var value = 0UL;
        for (var index = 0; index < 8; index++)
            value |= (ulong)frame[offset + index] << (8 * index);
        return new IeeeAddress(value);
    }
}
