namespace Haus.Zigbee.Models;

public record ZigbeeDeviceInfo(string ManufacturerName, string ModelIdentifier)
{
    public static ZigbeeDeviceInfo Empty { get; } = new(string.Empty, string.Empty);
}
