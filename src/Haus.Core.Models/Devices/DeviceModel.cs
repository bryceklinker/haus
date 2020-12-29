using System;
using Haus.Core.Models.Common;

namespace Haus.Core.Models.Devices
{
    public class DeviceModel : IModel
    {
        public long Id { get; set; }
        public long? RoomId { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public DeviceType DeviceType { get; set; }
        public DeviceMetadataModel[] Metadata { get; set; } = Array.Empty<DeviceMetadataModel>();
    }
}