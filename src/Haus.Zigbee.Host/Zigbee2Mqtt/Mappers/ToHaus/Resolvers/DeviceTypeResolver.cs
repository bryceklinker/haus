using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;

public interface IDeviceTypeResolver
{
    DeviceType Resolve(Zigbee2MqttMeta metadata);
    DeviceType Resolve(string vendor, string model);
}

public class DeviceTypeResolver(IOptions<HausOptions> options) : IDeviceTypeResolver
{
    private static readonly Lazy<DeviceTypeOptions[]> DefaultDeviceTypeOptions = new(LoadDefaultDeviceTypeOptions);

    private IEnumerable<DeviceTypeOptions> DeviceTypeOptions => options.Value.DeviceTypeOptions;

    private static IEnumerable<DeviceTypeOptions> DefaultOptions => DefaultDeviceTypeOptions.Value;

    public DeviceType Resolve(Zigbee2MqttMeta metadata)
    {
        return Resolve(metadata.Vendor, metadata.Model);
    }

    public DeviceType Resolve(string vendor, string model)
    {
        var match = GetDeviceTypeFromOptions(vendor, model) ?? GetDeviceTypeFromDefaults(vendor, model);
        return match?.DeviceType ?? DeviceType.Unknown;
    }

    private static DeviceTypeOptions[] LoadDefaultDeviceTypeOptions()
    {
        var provider = new EmbeddedFileProvider(typeof(DeviceTypeResolver).Assembly);
        using var stream = provider.GetFileInfo("Zigbee2Mqtt/Mappers/ToHaus/Resolvers/DefaultDeviceTypeOptions.json")
            .CreateReadStream();
        using var reader = new StreamReader(stream);
        return HausJsonSerializer.Deserialize<DeviceTypeOptions[]>(reader.ReadToEnd());
    }

    private DeviceTypeOptions GetDeviceTypeFromOptions(string vendor, string model)
    {
        return GetDeviceTypeOptionsFromSet(vendor, model, DeviceTypeOptions);
    }

    private static DeviceTypeOptions GetDeviceTypeFromDefaults(string vendor, string model)
    {
        return GetDeviceTypeOptionsFromSet(vendor, model, DefaultOptions);
    }

    private static DeviceTypeOptions GetDeviceTypeOptionsFromSet(
        string vendor,
        string model,
        IEnumerable<DeviceTypeOptions> set)
    {
        return set
            .FirstOrDefault(d => d.Matches(vendor, model));
    }
}