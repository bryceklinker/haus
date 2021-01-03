using System.Collections.Generic;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee
{
    public class HausLightingToZigbeeMapper : IToZigbeeMapper
    {
        private readonly IOptions<ZigbeeOptions> _options;

        private string ZigbeeBaseTopic => _options.Value.Config.Mqtt.BaseTopic;

        public HausLightingToZigbeeMapper(IOptions<ZigbeeOptions> options)
        {
            _options = options;
        }
        
        public bool IsSupported(string type)
        {
            return type == DeviceLightingChangedEvent.Type;
        }

        public IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message)
        {
            var command = HausJsonSerializer.Deserialize<HausCommand<DeviceLightingChangedEvent>>(message.Payload);
            var device = command.Payload.Device;
            yield return new MqttApplicationMessage
            {
                Topic = $"{ZigbeeBaseTopic}/{device.ExternalId}/set",
                Payload = CreateLightingPayload(command.Payload.Lighting)
            };

        }

        private static byte[] CreateLightingPayload(LightingModel lighting)
        {
            return HausJsonSerializer.SerializeToBytes(new
            {
                state = lighting.State.ToString().ToUpperInvariant(),
                brightness = lighting.Level,
                color_temp = lighting.Temperature,
                color = lighting.Color == null
                    ? null
                    : new
                    {
                        b = lighting.Color.Blue,
                        g = lighting.Color.Green,
                        r = lighting.Color.Red
                    }
            });
        }
    }
}