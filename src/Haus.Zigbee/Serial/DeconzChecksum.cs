using System;
using System.Buffers.Binary;

namespace Haus.Zigbee.Serial;

public static class DeconzChecksum
{
    public static ushort Compute(ReadOnlySpan<byte> frame)
    {
        var sum = 0;
        foreach (var value in frame)
            sum += value;

        return (ushort)((~sum + 1) & UInt16Mask);
    }

    private const int ChecksumByteLength = 2;
    private const int UInt16Mask = 0xFFFF;

    public static bool IsValid(ReadOnlySpan<byte> frameWithChecksum)
    {
        if (frameWithChecksum.Length < ChecksumByteLength)
            return false;

        var frame = frameWithChecksum[..^2];
        var trailingChecksum = BinaryPrimitives.ReadUInt16LittleEndian(frameWithChecksum[^2..]);

        return Compute(frame) == trailingChecksum;
    }
}
