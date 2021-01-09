using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Models.DeviceSimulator
{
    public record SimulatedDeviceModel(string Id = null, DeviceType DeviceType = DeviceType.Unknown, MetadataModel[] Metadata = null, LightingModel Lighting = null)
    {
        public MetadataModel[] Metadata { get; } = Metadata ?? Array.Empty<MetadataModel>();
        public LightingModel Lighting { get; } = DeviceType == DeviceType.Light ? Lighting : null;
    }
}