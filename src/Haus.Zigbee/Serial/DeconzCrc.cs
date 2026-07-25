using System;

namespace Haus.Zigbee.Serial;

public static class DeconzCrc
{
    public static ushort Compute(ReadOnlySpan<byte> frame)
    {
        var sum = 0;
        foreach (var value in frame)
            sum += value;

        return (ushort)((~sum + 1) & 0xFFFF);
    }
}
