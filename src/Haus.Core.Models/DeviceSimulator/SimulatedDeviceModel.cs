using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;

namespace Haus.Core.Models.DeviceSimulator
{
    public record SimulatedDeviceModel(string Id = null, DeviceType DeviceType = DeviceType.Unknown, MetadataModel[] Metadata = null)
    {
        public MetadataModel[] Metadata { get; } = Metadata ?? Array.Empty<MetadataModel>();
    }
}