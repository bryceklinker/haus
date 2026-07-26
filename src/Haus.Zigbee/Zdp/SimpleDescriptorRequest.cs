using System;
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
        return new[]
        {
            request.TransactionSequenceNumber,
            (byte)(request.NetworkAddress & 0xff),
            (byte)(request.NetworkAddress >> 8),
            request.Endpoint,
        };
    }

    public static SimpleDescriptorResponse DecodeResponse(ReadOnlySpan<byte> payload)
    {
        var transactionSequenceNumber = payload[0];
        var status = (ZdoStatus)payload[1];
        var descriptor = status == ZdoStatus.Success ? DecodeDescriptor(payload) : null;
        return new SimpleDescriptorResponse(transactionSequenceNumber, status, descriptor);
    }

    private static SimpleDescriptor DecodeDescriptor(ReadOnlySpan<byte> payload)
    {
        var inClusters = ReadClusterList(payload, 11, out var afterInClusters);
        var outClusters = ReadClusterList(payload, afterInClusters, out _);
        return new SimpleDescriptor(
            NetworkAddress: ReadUInt16(payload, 2),
            Endpoint: payload[5],
            ProfileId: ReadUInt16(payload, 6),
            DeviceId: ReadUInt16(payload, 8),
            DeviceVersion: payload[10],
            inClusters,
            outClusters
        );
    }

    private static IReadOnlyList<ushort> ReadClusterList(ReadOnlySpan<byte> payload, int offset, out int nextOffset)
    {
        var count = payload[offset];
        var clusters = new ushort[count];
        for (var index = 0; index < count; index++)
        {
            clusters[index] = ReadUInt16(payload, offset + 1 + (index * 2));
        }

        nextOffset = offset + 1 + (count * 2);
        return clusters;
    }

    private static ushort ReadUInt16(ReadOnlySpan<byte> payload, int offset)
    {
        return (ushort)(payload[offset] | (payload[offset + 1] << 8));
    }
}
