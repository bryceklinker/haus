using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Discovery
{
    public class DeviceDiscoveredModel : IHausEventConverter<DeviceDiscoveredModel>
    {
        public const string Type = "device_discovered";
        public string Id { get; set; }
        public string Model { get; set; }
        public string Vendor { get; set; }
        public string Description { get; set; }
        public DeviceType DeviceType { get; set; }

        public HausEvent<DeviceDiscoveredModel> AsHausEvent()
        {
            return new HausEvent<DeviceDiscoveredModel>(Type, this);
        }
    }
}