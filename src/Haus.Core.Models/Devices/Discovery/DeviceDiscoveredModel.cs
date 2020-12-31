using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Discovery
{
    public record DeviceDiscoveredModel : IHausEventCreator<DeviceDiscoveredModel>
    {
        public const string Type = "device_discovered";
        public string Id { get; }
        public DeviceType DeviceType { get; }
        public MetadataModel[] Metadata { get; }

        public DeviceDiscoveredModel(string id, DeviceType deviceType = DeviceType.Unknown, MetadataModel[] metadata = null)
        {
            Id = id;
            DeviceType = deviceType;
            Metadata = metadata ?? Array.Empty<MetadataModel>();
        }
        
        public HausEvent<DeviceDiscoveredModel> AsHausEvent()
        {
            return new(Type, this);
        }
    }
}