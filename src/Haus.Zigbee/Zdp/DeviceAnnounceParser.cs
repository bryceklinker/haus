using System;
using System.Buffers.Binary;
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
    private const int CapabilitiesOffset = 11;

    public static DeviceAnnounce Parse(ReadOnlySpan<byte> payload)
    {
        return new DeviceAnnounce(
            payload[TransactionSequenceNumberOffset],
            BinaryPrimitives.ReadUInt16LittleEndian(payload[NetworkAddressOffset..]),
            new IeeeAddress(BinaryPrimitives.ReadUInt64LittleEndian(payload[IeeeAddressOffset..])),
            payload[CapabilitiesOffset]
        );
    }
}
