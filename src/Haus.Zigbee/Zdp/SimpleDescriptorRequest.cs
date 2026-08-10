using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Haus.Zigbee.Zdp;

public record SimpleDescriptorRequest(byte TransactionSequenceNumber, ushort NetworkAddress, byte Endpoint);

public record SimpleDescriptor(
    ushort NetworkAddress,
    byte Endpoint,
    ushort ProfileId,
    ushort DeviceId,
    byte DeviceVersion,
    IReadOnlyList<ushort> InClusters,
    IReadOnlyList<ushort> OutClusters
);

public record SimpleDescriptorResponse(byte TransactionSequenceNumber, ZdoStatus Status, SimpleDescriptor? Descriptor);

public static class SimpleDescriptorCodec
{
    public static byte[] EncodeRequest(SimpleDescriptorRequest request)
    {
        var bytes = new byte[4];
        bytes[0] = request.TransactionSequenceNumber;
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(1), request.NetworkAddress);
        bytes[3] = request.Endpoint;
        return bytes;
    }

    private const int TransactionSequenceNumberOffset = 0;
    private const int StatusOffset = 1;
    private const int NetworkAddressOffset = 2;
    private const int EndpointOffset = 5;
    private const int ProfileIdOffset = 6;
    private const int DeviceIdOffset = 8;
    private const int DeviceVersionOffset = 10;
    private const int InClusterCountOffset = 11;
    private const int FixedDescriptorFieldsLength = 11;
    private const int ClusterIdByteLength = 2;

    // This response comes straight off the wire, so a truncated payload -- including one whose
    // in/out cluster count claims more cluster ids than are actually present -- must produce a
    // null result here rather than throw. See ZclFrameHeaderCodec.Decode for why an exception
    // here would silently stop delivery to every other IndicationReceived subscriber.
    public static SimpleDescriptorResponse? DecodeResponse(ReadOnlySpan<byte> payload)
    {
        if (payload.Length <= StatusOffset)
            return null;

        var transactionSequenceNumber = payload[TransactionSequenceNumberOffset];
        var status = (ZdoStatus)payload[StatusOffset];
        if (status != ZdoStatus.Success)
            return new SimpleDescriptorResponse(transactionSequenceNumber, status, null);

        var descriptor = TryDecodeDescriptor(payload);
        return descriptor is null ? null : new SimpleDescriptorResponse(transactionSequenceNumber, status, descriptor);
    }

    private static SimpleDescriptor? TryDecodeDescriptor(ReadOnlySpan<byte> payload)
    {
        if (payload.Length <= FixedDescriptorFieldsLength)
            return null;

        var inClusters = TryReadClusterList(payload, InClusterCountOffset, out var afterInClusters);
        if (inClusters is null)
            return null;

        var outClusters = TryReadClusterList(payload, afterInClusters, out _);
        if (outClusters is null)
            return null;

        return new SimpleDescriptor(
            NetworkAddress: BinaryPrimitives.ReadUInt16LittleEndian(payload[NetworkAddressOffset..]),
            Endpoint: payload[EndpointOffset],
            ProfileId: BinaryPrimitives.ReadUInt16LittleEndian(payload[ProfileIdOffset..]),
            DeviceId: BinaryPrimitives.ReadUInt16LittleEndian(payload[DeviceIdOffset..]),
            DeviceVersion: payload[DeviceVersionOffset],
            inClusters,
            outClusters
        );
    }

    private static IReadOnlyList<ushort>? TryReadClusterList(ReadOnlySpan<byte> payload, int offset, out int nextOffset)
    {
        nextOffset = offset;
        if (payload.Length <= offset)
            return null;

        var count = payload[offset];
        var afterCount = offset + 1;
        var clusterBytesLength = count * ClusterIdByteLength;
        if (payload.Length < afterCount + clusterBytesLength)
            return null;

        var clusters = new ushort[count];
        for (var index = 0; index < count; index++)
        {
            clusters[index] = BinaryPrimitives.ReadUInt16LittleEndian(
                payload[(afterCount + (index * ClusterIdByteLength))..]
            );
        }

        nextOffset = afterCount + clusterBytesLength;
        return clusters;
    }
}
