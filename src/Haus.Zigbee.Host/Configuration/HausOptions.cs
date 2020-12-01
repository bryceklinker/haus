using System;
using Haus.Core.Models.Devices;

namespace Haus.Zigbee.Host.Configuration
{
    public class HausOptions
    {
        public string Server { get; set; }
        public string EventsTopic { get; set; }
        public string CommandsTopic { get; set; }
        public string UnknownTopic { get; set; }

        public DeviceTypeOptions[] DeviceTypeOptions { get; set; } = Array.Empty<DeviceTypeOptions>();
    }
}