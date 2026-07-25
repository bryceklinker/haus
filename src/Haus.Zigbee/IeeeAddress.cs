using System;
using System.Globalization;

namespace Haus.Zigbee;

public readonly record struct IeeeAddress(ulong Value)
{
    private const string HexPrefix = "0x";

    public static bool TryParse(string? text, out IeeeAddress address)
    {
        address = default;
        var hasPrefix = text?.StartsWith(HexPrefix, StringComparison.Ordinal) == true;
        if (!hasPrefix)
            return false;

        var hexDigits = text!.Substring(HexPrefix.Length);
        var isParsed = ulong.TryParse(hexDigits, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value);
        if (!isParsed)
            return false;

        address = new IeeeAddress(value);
        return true;
    }

    public override string ToString()
    {
        return $"0x{Value:x16}";
    }
}
