using System;
using System.Globalization;

namespace Haus.Zigbee;

public readonly record struct IeeeAddress(ulong Value)
{
    private const string HexPrefix = "0x";
    private const int HexDigitCount = 16;

    public static bool TryParse(string? text, out IeeeAddress address)
    {
        address = default;
        if (text is null || !text.StartsWith(HexPrefix, StringComparison.Ordinal))
            return false;

        var hexDigits = text.Substring(HexPrefix.Length);
        if (hexDigits.Length != HexDigitCount)
            return false;

        if (!ulong.TryParse(hexDigits, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            return false;

        address = new IeeeAddress(value);
        return true;
    }

    public override string ToString()
    {
        return $"{HexPrefix}{Value:x16}";
    }
}
