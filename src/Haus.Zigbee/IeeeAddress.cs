namespace Haus.Zigbee;

public readonly record struct IeeeAddress(ulong Value)
{
    public override string ToString()
    {
        return $"0x{Value:x16}";
    }
}
