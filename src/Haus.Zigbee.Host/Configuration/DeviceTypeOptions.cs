using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Configuration
{
    public class DeviceTypeOptions
    {
        public string Vendor { get; set; }
        public string Model { get; set; }
        public DeviceType DeviceType { get; set; }

        public DeviceTypeOptions(string vendor = null, string model = null, DeviceType deviceType = DeviceType.Unknown)
        {
            Model = model;
            Vendor = vendor;
            DeviceType = deviceType;
        }

        public bool Matches(Zigbee2MqttMeta meta)
        {
            return Matches(meta.Model, meta.Vendor);
        }

        public bool Matches(DeviceTypeOptions options)
        {
            return Matches(options.Vendor, options.Model);
        }

        public bool Matches(string vendor, string model)
        {
            return Model == model
                && Vendor == vendor;
        }
    }
}