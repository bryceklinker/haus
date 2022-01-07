using Haus.Core.Models.Devices;

namespace Haus.Zigbee.Host.Configuration;

public record DeviceTypeOptions(string Vendor = null, string Model = null, DeviceType DeviceType = DeviceType.Unknown)
{
    public bool Matches(string vendor, string model)
    {
        return Model == model
               && Vendor == vendor;
    }
}