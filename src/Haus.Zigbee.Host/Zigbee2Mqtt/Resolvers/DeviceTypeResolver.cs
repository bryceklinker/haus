using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Resolvers
{
    public interface IDeviceTypeResolver
    {
        DeviceType Resolve(Zigbee2MqttMeta metadata);
    }
    
    public class DeviceTypeResolver : IDeviceTypeResolver
    {
        private readonly IOptionsMonitor<HausOptions> _optionsMonitor;
        private static readonly Lazy<DeviceTypeOptions[]> DefaultDeviceTypeOptions = new Lazy<DeviceTypeOptions[]>(LoadDefaultDeviceTypeOptions);

        private IEnumerable<DeviceTypeOptions> DeviceTypeOptions => _optionsMonitor.CurrentValue.DeviceTypeOptions;
        
        public DeviceTypeResolver(IOptionsMonitor<HausOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        private static IEnumerable<DeviceTypeOptions> DefaultOptions => DefaultDeviceTypeOptions.Value;
        
        public DeviceType Resolve(Zigbee2MqttMeta metadata)
        {
            var match = GetDeviceTypeFromOptions(metadata) ?? GetDeviceTypeFromDefaults(metadata);   
            return match?.DeviceType ?? DeviceType.Unknown;
        }

        private static DeviceTypeOptions[] LoadDefaultDeviceTypeOptions()
        {
            var provider = new EmbeddedFileProvider(typeof(DeviceTypeResolver).Assembly);
            using var stream = provider.GetFileInfo("Zigbee2Mqtt/Resolvers/DefaultDeviceTypeOptions.json").CreateReadStream();
            using var reader = new StreamReader(stream);
            return JsonSerializer.Deserialize<DeviceTypeOptions[]>(reader.ReadToEnd());
        }

        private DeviceTypeOptions GetDeviceTypeFromOptions(Zigbee2MqttMeta metadata)
        {
            return GetDeviceTypeOptionsFromSet(metadata, DeviceTypeOptions);
        }

        private static DeviceTypeOptions GetDeviceTypeFromDefaults(Zigbee2MqttMeta metadata)
        {
            return GetDeviceTypeOptionsFromSet(metadata, DefaultOptions);
        }

        private static DeviceTypeOptions GetDeviceTypeOptionsFromSet(
            Zigbee2MqttMeta meta,
            IEnumerable<DeviceTypeOptions> set)
        {
            return set
                .FirstOrDefault(d => d.Matches(meta));
        }
    }
}