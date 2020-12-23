using System;
using Haus.Core.Models.Devices;
using Haus.Mqtt.Client.Settings;

namespace Haus.Zigbee.Host.Configuration
{
    public class HausOptions : HausMqttSettings
    {
        public DeviceTypeOptions[] DeviceTypeOptions { get; set; } = Array.Empty<DeviceTypeOptions>();
    }
}