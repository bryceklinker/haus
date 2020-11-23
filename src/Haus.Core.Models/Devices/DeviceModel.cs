using System.Collections.Generic;

namespace Haus.Core.Models.Devices
{
    public class DeviceModel
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        public DeviceMetadataModel[] Metadata { get; set; }
    }

    public class DeviceMetadataModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}