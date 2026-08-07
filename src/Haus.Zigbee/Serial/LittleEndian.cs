using System.Collections.Generic;

namespace Haus.Zigbee.Serial;

public static class LittleEndian
{
    public static void Write(List<byte> bytes, ushort value)
    {
        bytes.Add((byte)(value & 0xff));
        bytes.Add((byte)(value >> 8));
    }

    public static void Write(List<byte> bytes, ulong value)
    {
        for (var shift = 0; shift < 64; shift += 8)
            bytes.Add((byte)((value >> shift) & 0xff));
    }

    public static byte[] Bytes(ushort value) => [(byte)value, (byte)(value >> 8)];
}
