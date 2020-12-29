using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;

namespace Haus.Core.Models.DeviceSimulator
{
    public class CreateSimulatedDeviceModel
    {
        public DeviceType DeviceType { get; set; } = DeviceType.Unknown;
        public MetadataModel[] Metadata { get; set; } = Array.Empty<MetadataModel>();
    }
}