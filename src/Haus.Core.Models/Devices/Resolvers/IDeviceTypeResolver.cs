using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Haus.Core.Models.Devices.Resolvers;

public interface IDeviceTypeResolver
{
    DeviceType Resolve(string? vendor, string? model);
}

public class DeviceTypeResolver(IEnumerable<DeviceTypeOptions>? customDeviceTypeOptions = null) : IDeviceTypeResolver
{
    private static readonly Lazy<DeviceTypeOptions[]> DefaultDeviceTypeOptions = new(LoadDefaultDeviceTypeOptions);

    private readonly DeviceTypeOptions[] _customDeviceTypeOptions = customDeviceTypeOptions?.ToArray() ?? [];

    private static IEnumerable<DeviceTypeOptions> DefaultOptions => DefaultDeviceTypeOptions.Value;

    public DeviceType Resolve(string? vendor, string? model)
    {
        var match =
            GetDeviceTypeOptionsFromSet(vendor, model, _customDeviceTypeOptions)
            ?? GetDeviceTypeOptionsFromSet(vendor, model, DefaultOptions);
        return match?.DeviceType ?? DeviceType.Unknown;
    }

    private static DeviceTypeOptions[] LoadDefaultDeviceTypeOptions()
    {
        var resourceName = $"{typeof(DeviceTypeResolver).Namespace}.DefaultDeviceTypeOptions.json";
        using var stream = typeof(DeviceTypeResolver).Assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return HausJsonSerializer.Deserialize<DeviceTypeOptions[]>(reader.ReadToEnd()) ?? [];
    }

    private static DeviceTypeOptions? GetDeviceTypeOptionsFromSet(
        string? vendor,
        string? model,
        IEnumerable<DeviceTypeOptions> set
    )
    {
        return set.FirstOrDefault(d => d.Matches(vendor, model));
    }
}
