using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Events
{
    public record DeviceDiscoveredEvent(string Id, DeviceType DeviceType = DeviceType.Unknown, MetadataModel[] Metadata = null) : IHausEventCreator<DeviceDiscoveredEvent>
    {
        public const string Type = "device_discovered";
        public MetadataModel[] Metadata { get; } = Metadata ?? Array.Empty<MetadataModel>();
        
        public HausEvent<DeviceDiscoveredEvent> AsHausEvent() => new(Type, this);
    }
}