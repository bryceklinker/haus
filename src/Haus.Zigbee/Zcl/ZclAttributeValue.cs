using System.Collections.Generic;

namespace Haus.Zigbee.Zcl;

public enum ZclDataType : byte
{
    Bool = 0x10,
    Uint16 = 0x21,
    Int16 = 0x29,
}

public static class ZclDataTypeWidths
{
    private static readonly IReadOnlyDictionary<byte, int> WidthsByType = new Dictionary<byte, int>
    {
        [(byte)ZclDataType.Bool] = 1,
        [(byte)ZclDataType.Uint16] = 2,
        [(byte)ZclDataType.Int16] = 2,
    };

    public static bool TryGetWidth(byte dataType, out int width) => WidthsByType.TryGetValue(dataType, out width);
}

public record ZclAttributeValue(ZclDataType DataType, ulong RawValue)
{
    public bool AsBool() => RawValue != 0;

    public ulong AsUnsigned() => RawValue;

    public long AsSigned()
    {
        ZclDataTypeWidths.TryGetWidth((byte)DataType, out var width);
        var bits = width * 8;
        if (bits >= 64)
        {
            return (long)RawValue;
        }

        var signBit = 1ul << (bits - 1);
        var isNegative = (RawValue & signBit) != 0;
        return isNegative ? (long)RawValue - (long)(1ul << bits) : (long)RawValue;
    }
}
