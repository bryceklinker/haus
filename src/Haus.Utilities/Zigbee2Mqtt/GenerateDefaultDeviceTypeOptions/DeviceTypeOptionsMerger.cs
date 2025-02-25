using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Configuration;

namespace Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;

public interface IDeviceTypeOptionsMerger
{
    IEnumerable<DeviceTypeOptions> Merge(
        IEnumerable<DeviceTypeOptions> existing,
        IEnumerable<DeviceTypeOptions> latest
    );
}

public class DeviceTypeOptionsMerger : IDeviceTypeOptionsMerger
{
    public IEnumerable<DeviceTypeOptions> Merge(
        IEnumerable<DeviceTypeOptions> existing,
        IEnumerable<DeviceTypeOptions> latest
    )
    {
        return existing
            .Concat(latest)
            .GroupBy(o => $"{o.Vendor}-{o.Model}", options => options)
            .Select(group =>
            {
                var model = group.First().Model;
                var vendor = group.First().Vendor;
                var options = group.FirstOrDefault(d => d.DeviceType != DeviceType.Unknown);
                return new DeviceTypeOptions(vendor, model, options?.DeviceType ?? DeviceType.Unknown);
            })
            .OrderBy(opts => opts.Vendor);
    }
}
