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

public record ApsDataIndicationFrame(
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
    private const int AddressModeAndBeyondOffset = 8;
    private const byte SuccessStatus = 0x00;
    private const int ReservedByteCount = 2;

    public static ApsDataIndicationFrame? Decode(ReadOnlySpan<byte> frame)
    {
        if (frame[StatusOffset] != SuccessStatus)
            return null;

        var deviceState = frame[DeviceStateOffset];
        var reader = new FrameReader(frame, AddressModeAndBeyondOffset);

        var destinationAddressMode = (DeconzAddressMode)reader.ReadByte();
        var isIeeeDestination = destinationAddressMode == DeconzAddressMode.Ieee;
        var destinationIeeeAddress = isIeeeDestination ? reader.ReadIeeeAddress() : default(IeeeAddress?);
        var destinationNwkAddress = isIeeeDestination ? default(ushort?) : reader.ReadUInt16();
        var destinationEndpoint = reader.ReadByte();

        var sourceAddressMode = (DeconzAddressMode)reader.ReadByte();
        if (!IsSupportedSourceMode(sourceAddressMode))
            return null;

        var sourceNwkAddress = reader.ReadUInt16();
        var hasIeeeSource = sourceAddressMode == DeconzAddressMode.NwkAndIeee;
        var sourceIeeeAddress = hasIeeeSource ? reader.ReadIeeeAddress() : default(IeeeAddress?);
        var sourceEndpoint = reader.ReadByte();

        var profileId = reader.ReadUInt16();
        var clusterId = reader.ReadUInt16();
        var asduLength = reader.ReadUInt16();
        var asduPayload = reader.ReadBytes(asduLength);

        reader.Skip(ReservedByteCount);
        var linkQualityIndicator = reader.ReadByte();
        var rssi = reader.HasRemaining ? (sbyte?)(sbyte)reader.ReadByte() : null;

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

    private static bool IsSupportedSourceMode(DeconzAddressMode mode)
    {
        return mode == DeconzAddressMode.Nwk || mode == DeconzAddressMode.NwkAndIeee;
    }

    private ref struct FrameReader(ReadOnlySpan<byte> frame, int offset)
    {
        private const int IeeeAddressByteCount = 8;

        private readonly ReadOnlySpan<byte> _frame = frame;
        private int _offset = offset;

        public readonly bool HasRemaining => _offset < _frame.Length;

        public byte ReadByte()
        {
            return _frame[_offset++];
        }

        public ushort ReadUInt16()
        {
            var value = (ushort)(_frame[_offset] | (_frame[_offset + 1] << 8));
            _offset += 2;
            return value;
        }

        public IeeeAddress ReadIeeeAddress()
        {
            var value = 0UL;
            for (var index = 0; index < IeeeAddressByteCount; index++)
                value |= (ulong)_frame[_offset + index] << (8 * index);
            _offset += IeeeAddressByteCount;
            return new IeeeAddress(value);
        }

        public byte[] ReadBytes(int count)
        {
            var bytes = _frame.Slice(_offset, count).ToArray();
            _offset += count;
            return bytes;
        }

        public void Skip(int count)
        {
            _offset += count;
        }
    }
}
