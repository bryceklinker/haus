using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Discovery;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers
{
    public class BridgeMessageMapper : IMapper
    {
        private readonly IOptions<HausOptions> _hausOptions;

        private string EventsTopic => _hausOptions.Value.EventsTopic;
        
        public BridgeMessageMapper(IOptions<HausOptions> hausOptions)
        {
            _hausOptions = hausOptions;
        }

        public MqttApplicationMessage Map(Zigbee2MqttMessage zigbeeMessage)
        {
            return new MqttApplicationMessage
            {
                Topic = EventsTopic,
                Payload = JsonSerializer.SerializeToUtf8Bytes(ToHausEvent(zigbeeMessage))
            };
        }
        
        private HausEvent ToHausEvent(Zigbee2MqttMessage message)
        {
            return new HausEvent
            {
                Type = DeviceDiscoveredModel.Type,
                Payload = JsonSerializer.Serialize(CreateDeviceDiscovered(message))
            };
        }

        private DeviceDiscoveredModel CreateDeviceDiscovered(Zigbee2MqttMessage message)
        {
            return new DeviceDiscoveredModel
            {
                Id = message.Meta.FriendlyName,
                Description = message.Meta.Description,
                Model = message.Meta.Model,
                Vendor = message.Meta.Vendor
            };
        }
    }
}