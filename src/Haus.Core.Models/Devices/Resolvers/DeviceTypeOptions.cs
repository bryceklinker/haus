namespace Haus.Core.Models.Devices.Resolvers;

public record DeviceTypeOptions(string? Vendor = null, string? Model = null, DeviceType DeviceType = DeviceType.Unknown)
{
    public bool Matches(string? vendor, string? model)
    {
        return Model == model && Vendor == vendor;
    }
}
