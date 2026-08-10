using System;
using System.Buffers.Binary;
using Haus.Zigbee.Models;

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
        if (frame.Length <= DeviceStateOffset)
            return null;
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

        // Every read above is bounds-checked by FrameReader instead of throwing, since this frame
        // comes straight off the wire -- see ZclFrameHeaderCodec.Decode for why an exception here
        // would silently stop delivery to every other IndicationReceived subscriber.
        if (reader.Failed)
            return null;

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
        private const int UInt16ByteCount = 2;

        private readonly ReadOnlySpan<byte> _frame = frame;
        private int _offset = offset;

        public bool Failed { get; private set; }

        public readonly bool HasRemaining => _offset < _frame.Length;

        public byte ReadByte()
        {
            return TryAdvance(1, out var start) ? _frame[start] : (byte)0;
        }

        public ushort ReadUInt16()
        {
            return TryAdvance(UInt16ByteCount, out var start)
                ? BinaryPrimitives.ReadUInt16LittleEndian(_frame[start..])
                : (ushort)0;
        }

        public IeeeAddress ReadIeeeAddress()
        {
            return TryAdvance(IeeeAddressByteCount, out var start)
                ? new IeeeAddress(BinaryPrimitives.ReadUInt64LittleEndian(_frame[start..]))
                : default;
        }

        public byte[] ReadBytes(int count)
        {
            return count >= 0 && TryAdvance(count, out var start) ? _frame.Slice(start, count).ToArray() : [];
        }

        public void Skip(int count)
        {
            TryAdvance(count, out _);
        }

        // Once a read fails there aren't enough bytes to trust the offset for any later read either,
        // so every subsequent call short-circuits to failure without touching the span again.
        private bool TryAdvance(int count, out int start)
        {
            start = _offset;
            if (Failed || start + count > _frame.Length)
            {
                Failed = true;
                return false;
            }

            _offset += count;
            return true;
        }
    }
}
