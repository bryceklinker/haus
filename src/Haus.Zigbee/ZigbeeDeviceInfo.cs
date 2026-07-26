namespace Haus.Zigbee;

public record ZigbeeDeviceInfo(string ManufacturerName, string ModelIdentifier)
{
    public static ZigbeeDeviceInfo Empty { get; } = new(string.Empty, string.Empty);
}
