using Haus.Core.Models;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Pairing
{
    public class DeviceDiscoveredMapper
    {
        private readonly IOptionsMonitor<HausOptions> _hausOptionsMonitor;
        private readonly IDeviceTypeResolver _deviceTypeResolver;

        private string EventsTopic => _hausOptionsMonitor.CurrentValue.EventsTopic;

        public DeviceDiscoveredMapper(IOptionsMonitor<HausOptions> hausOptionsMonitor)
        {
            _hausOptionsMonitor = hausOptionsMonitor;
            _deviceTypeResolver = new DeviceTypeResolver(_hausOptionsMonitor);
        }

        public MqttApplicationMessage Map(Zigbee2MqttMessage zigbeeMessage)
        {
            return new MqttApplicationMessage
            {
                Topic = EventsTopic,
                Payload = HausJsonSerializer.SerializeToBytes(new HausEvent<DeviceDiscoveredModel>
                {
                    Type = DeviceDiscoveredModel.Type,
                    Payload = new DeviceDiscoveredModel
                    {
                        Id = zigbeeMessage.Meta.FriendlyName,
                        Description = zigbeeMessage.Meta.Description,
                        Model = zigbeeMessage.Meta.Model,
                        Vendor = zigbeeMessage.Meta.Vendor,
                        DeviceType = _deviceTypeResolver.Resolve(zigbeeMessage.Meta)
                    }
                })
            };
        }
    }
}