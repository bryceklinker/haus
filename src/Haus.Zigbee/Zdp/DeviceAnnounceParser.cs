using System;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Zdp;

public record DeviceAnnounce(
    byte TransactionSequenceNumber,
    ushort NetworkAddress,
    IeeeAddress IeeeAddress,
    byte Capabilities
);

public static class DeviceAnnounceParser
{
    private const int TransactionSequenceNumberOffset = 0;
    private const int NetworkAddressOffset = 1;
    private const int IeeeAddressOffset = 3;
    private const int IeeeAddressByteCount = 8;
    private const int CapabilitiesOffset = 11;

    public static DeviceAnnounce Parse(ReadOnlySpan<byte> payload)
    {
        return new DeviceAnnounce(
            payload[TransactionSequenceNumberOffset],
            ReadUInt16(payload, NetworkAddressOffset),
            ReadIeeeAddress(payload, IeeeAddressOffset),
            payload[CapabilitiesOffset]
        );
    }

    private static ushort ReadUInt16(ReadOnlySpan<byte> payload, int offset)
    {
        return (ushort)(payload[offset] | (payload[offset + 1] << 8));
    }

    private static IeeeAddress ReadIeeeAddress(ReadOnlySpan<byte> payload, int offset)
    {
        var value = 0UL;
        for (var index = 0; index < IeeeAddressByteCount; index++)
            value |= (ulong)payload[offset + index] << (8 * index);
        return new IeeeAddress(value);
    }
}
