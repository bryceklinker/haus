using System.Collections.Generic;

namespace Haus.Zigbee.Zcl;

public enum ZclDataType : byte
{
    Bool = 0x10,
    Uint16 = 0x21,
}

public static class ZclDataTypeWidths
{
    private static readonly IReadOnlyDictionary<byte, int> WidthsByType = new Dictionary<byte, int>
    {
        [(byte)ZclDataType.Bool] = 1,
        [(byte)ZclDataType.Uint16] = 2,
    };

    public static bool TryGetWidth(byte dataType, out int width) => WidthsByType.TryGetValue(dataType, out width);
}

public sealed record ZclAttributeValue(ZclDataType DataType, ulong RawValue)
{
    public bool AsBool() => RawValue != 0;

    public ulong AsUnsigned() => RawValue;
}
