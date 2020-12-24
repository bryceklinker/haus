using System;
using System.Linq;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Discovery
{
    public class DeviceDiscoveredModel : IHausEventCreator<DeviceDiscoveredModel>
    {
        public const string Type = "device_discovered";
        public string Id { get; set; }
        public DeviceType DeviceType { get; set; } = DeviceType.Unknown;
        public DeviceMetadataModel[] Metadata { get; set; } = Array.Empty<DeviceMetadataModel>();
        
        public HausEvent<DeviceDiscoveredModel> AsHausEvent()
        {
            return new(Type, this);
        }

        public string GetMetadataValue(string key) => 
            Metadata.FirstOrDefault(m => m.Key == key)
            ?.Value;
    }
}