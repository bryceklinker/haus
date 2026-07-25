using System;
using System.Collections.Generic;

namespace Haus.Zigbee.Zdp;

public sealed record SimpleDescriptorRequest(byte TransactionSequenceNumber, ushort NetworkAddress, byte Endpoint);

public sealed record SimpleDescriptor(
    ushort NetworkAddress,
    byte Endpoint,
    ushort ProfileId,
    ushort DeviceId,
    byte DeviceVersion,
    IReadOnlyList<ushort> InClusters,
    IReadOnlyList<ushort> OutClusters
);

public sealed record SimpleDescriptorResponse(
    byte TransactionSequenceNumber,
    ZdoStatus Status,
    SimpleDescriptor? Descriptor
);

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
        if (status != ZdoStatus.Success)
        {
            return new SimpleDescriptorResponse(transactionSequenceNumber, status, Descriptor: null);
        }

        var networkAddress = ReadUInt16(payload, 2);
        var endpoint = payload[5];
        var profileId = ReadUInt16(payload, 6);
        var deviceId = ReadUInt16(payload, 8);
        var deviceVersion = payload[10];

        var inClusters = ReadClusterList(payload, 11, out var afterInClusters);
        var outClusters = ReadClusterList(payload, afterInClusters, out _);

        var descriptor = new SimpleDescriptor(
            networkAddress,
            endpoint,
            profileId,
            deviceId,
            deviceVersion,
            inClusters,
            outClusters
        );
        return new SimpleDescriptorResponse(transactionSequenceNumber, status, descriptor);
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
