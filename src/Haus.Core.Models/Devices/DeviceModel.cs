using System;

namespace Haus.Core.Models.Devices
{
    public class DeviceModel
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public DeviceMetadataModel[] Metadata { get; set; } = Array.Empty<DeviceMetadataModel>();
    }
}