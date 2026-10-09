using System;
using System.Buffers.Binary;

namespace Haus.Zigbee.Zdp;

public record NodeDescriptorRequest(byte TransactionSequenceNumber, ushort NetworkAddressOfInterest);

public record NodeDescriptorResponse(
    byte TransactionSequenceNumber,
    ZdoStatus Status,
    ushort NetworkAddressOfInterest,
    byte Capabilities
)
{
    public bool IsSleepy => !MacCapabilityFlags.IsReceiverOnWhenIdle(Capabilities);
}

public static class NodeDescriptorRequestCodec
{
    private const int RequestLength = 3;
    private const int TransactionSequenceNumberOffset = 0;
    private const int NetworkAddressOfInterestOffset = 1;

    public static byte[] Encode(NodeDescriptorRequest request)
    {
        var bytes = new byte[RequestLength];
        bytes[TransactionSequenceNumberOffset] = request.TransactionSequenceNumber;
        BinaryPrimitives.WriteUInt16LittleEndian(
            bytes.AsSpan(NetworkAddressOfInterestOffset),
            request.NetworkAddressOfInterest
        );
        return bytes;
    }
}

public static class NodeDescriptorResponseCodec
{
    private const int TransactionSequenceNumberOffset = 0;
    private const int StatusOffset = 1;
    private const int NetworkAddressOfInterestOffset = 2;
    private const int MacCapabilityFlagsOffset = 6;

    // This response comes straight off the wire, so a truncated payload must produce a null result
    // here rather than throw -- see NwkAddrResponseCodec.Decode for why an exception here would
    // silently stop delivery to every other IndicationReceived subscriber. Trailing Node Descriptor
    // fields past the MAC Capability Flags byte (manufacturer code, buffer sizes, server mask, ...)
    // are not needed for sleepy detection and are not decoded.
    public static NodeDescriptorResponse? Decode(ReadOnlySpan<byte> payload)
    {
        if (payload.Length <= StatusOffset)
            return null;

        var transactionSequenceNumber = payload[TransactionSequenceNumberOffset];
        var status = (ZdoStatus)payload[StatusOffset];
        if (status != ZdoStatus.Success)
            return new NodeDescriptorResponse(
                transactionSequenceNumber,
                status,
                NetworkAddressOfInterest: 0,
                Capabilities: 0
            );

        if (payload.Length <= MacCapabilityFlagsOffset)
            return null;

        var networkAddressOfInterest = BinaryPrimitives.ReadUInt16LittleEndian(
            payload[NetworkAddressOfInterestOffset..]
        );
        var capabilities = payload[MacCapabilityFlagsOffset];
        return new NodeDescriptorResponse(transactionSequenceNumber, status, networkAddressOfInterest, capabilities);
    }
}
