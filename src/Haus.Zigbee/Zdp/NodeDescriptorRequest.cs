using System;

namespace Haus.Zigbee.Zdp;

public sealed record NodeDescriptorRequest(byte TransactionSequenceNumber, ushort NetworkAddress);

public static class NodeDescriptorRequestCodec
{
    public static byte[] Encode(NodeDescriptorRequest request)
    {
        return new[]
        {
            request.TransactionSequenceNumber,
            (byte)(request.NetworkAddress & 0xff),
            (byte)(request.NetworkAddress >> 8),
        };
    }
}

public enum LogicalType : byte
{
    Coordinator = 0,
    Router = 1,
    EndDevice = 2,
}

public sealed record NodeDescriptor(
    LogicalType LogicalType,
    bool FragmentationSupported,
    byte ApsFlags,
    byte FrequencyBand,
    byte MacCapabilityFlags,
    ushort ManufacturerCode,
    byte MaxBufferSize,
    ushort MaxIncomingTransferSize,
    ushort ServerMask,
    ushort MaxOutgoingTransferSize,
    byte Deprecated1
);

public sealed record NodeDescriptorResponse(
    byte TransactionSequenceNumber,
    ZdoStatus Status,
    ushort NetworkAddress,
    NodeDescriptor? Descriptor
);

public static class NodeDescriptorResponseCodec
{
    private const int LogicalTypeMask = 0x07;
    private const int FragmentationSupportedBit = 0x20;
    private const int ApsFlagsMask = 0x07;
    private const int FrequencyBandMask = 0xf8;
    private const int FrequencyBandShift = 3;

    public static NodeDescriptorResponse Decode(ReadOnlySpan<byte> payload)
    {
        var reader = new PayloadReader(payload);
        var transactionSequenceNumber = reader.ReadByte();
        var status = (ZdoStatus)reader.ReadByte();
        if (status != ZdoStatus.Success)
            return new NodeDescriptorResponse(transactionSequenceNumber, status, NetworkAddress: 0, Descriptor: null);

        var networkAddress = reader.ReadUInt16();
        var descriptor = ReadDescriptor(ref reader);
        return new NodeDescriptorResponse(transactionSequenceNumber, status, networkAddress, descriptor);
    }

    private static NodeDescriptor ReadDescriptor(ref PayloadReader reader)
    {
        var byte1 = reader.ReadByte();
        var byte2 = reader.ReadByte();
        return new NodeDescriptor(
            LogicalType: (LogicalType)(byte1 & LogicalTypeMask),
            FragmentationSupported: (byte1 & FragmentationSupportedBit) != 0,
            ApsFlags: (byte)(byte2 & ApsFlagsMask),
            FrequencyBand: (byte)((byte2 & FrequencyBandMask) >> FrequencyBandShift),
            MacCapabilityFlags: reader.ReadByte(),
            ManufacturerCode: reader.ReadUInt16(),
            MaxBufferSize: reader.ReadByte(),
            MaxIncomingTransferSize: reader.ReadUInt16(),
            ServerMask: reader.ReadUInt16(),
            MaxOutgoingTransferSize: reader.ReadUInt16(),
            Deprecated1: reader.ReadByte()
        );
    }

    private ref struct PayloadReader(ReadOnlySpan<byte> payload)
    {
        private readonly ReadOnlySpan<byte> _payload = payload;
        private int _offset = 0;

        public byte ReadByte()
        {
            return _payload[_offset++];
        }

        public ushort ReadUInt16()
        {
            var value = (ushort)(_payload[_offset] | (_payload[_offset + 1] << 8));
            _offset += 2;
            return value;
        }
    }
}
