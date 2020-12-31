using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;

namespace Haus.Core.Models.DeviceSimulator
{
    public record CreateSimulatedDeviceModel
    {
        public DeviceType DeviceType { get; }
        public MetadataModel[] Metadata { get; }

        public CreateSimulatedDeviceModel(DeviceType deviceType = DeviceType.Unknown, MetadataModel[] metadata = null)
        {
            DeviceType = deviceType;
            Metadata = metadata ?? Array.Empty<MetadataModel>();
        }
    }
}