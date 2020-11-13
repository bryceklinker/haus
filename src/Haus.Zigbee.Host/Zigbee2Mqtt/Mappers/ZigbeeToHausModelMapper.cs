using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Discovery;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers
{
    public interface IZigbeeToHausModelMapper
    {
        MqttApplicationMessage ToHausEvent(MqttApplicationMessage message);
    }

    public class ZigbeeToHausModelMapper : IZigbeeToHausModelMapper
    {
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
        private readonly IOptions<HausOptions> _hausOptions;

        private string EventsTopic => _hausOptions.Value.EventsTopic;
        
        public ZigbeeToHausModelMapper(IOptions<ZigbeeOptions> zigbeeOptions, IOptions<HausOptions> hausOptions)
        {
            _zigbeeOptions = zigbeeOptions;
            _hausOptions = hausOptions;
        }

        public MqttApplicationMessage ToHausEvent(MqttApplicationMessage message)
        {
            var payload = new Zigbee2MqttPayload(message.Topic, message.Payload);
            return new MqttApplicationMessage
            {
                Topic = EventsTopic,
                Payload = JsonSerializer.SerializeToUtf8Bytes(ToHausEvent(payload))
            };
        }

        private HausEvent ToHausEvent(Zigbee2MqttPayload payload)
        {
            return new HausEvent
            {
                Type = DeviceDiscoveredModel.Type,
                Payload = JsonSerializer.Serialize(CreateDeviceDiscovered(payload))
            };
        }

        public DeviceDiscoveredModel CreateDeviceDiscovered(Zigbee2MqttPayload payload)
        {
            return new DeviceDiscoveredModel
            {
                Id = payload.Meta.FriendlyName
            };
        }
    }
}