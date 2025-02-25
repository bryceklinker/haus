using System;
using System.Collections.Generic;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;

public class HausLightingToZigbeeMapper(IOptions<ZigbeeOptions> options) : IToZigbeeMapper
{
    private string ZigbeeBaseTopic => options.Value.Config.Mqtt.BaseTopic;

    public bool IsSupported(string type)
    {
        return type == DeviceLightingChangedEvent.Type;
    }

    public IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message)
    {
        var command = HausJsonSerializer.Deserialize<HausCommand<DeviceLightingChangedEvent>>(message.PayloadSegment);
        var device = command.Payload.Device;
        yield return new MqttApplicationMessage
        {
            Topic = $"{ZigbeeBaseTopic}/{device.ExternalId}/set",
            PayloadSegment = CreateLightingPayload(command.Payload.Lighting),
        };
    }

    private static ArraySegment<byte> CreateLightingPayload(LightingModel lighting)
    {
        if (lighting.State == LightingState.Off)
            return HausJsonSerializer.SerializeToBytes(new { state = lighting.State.ToString().ToUpperInvariant() });

        return HausJsonSerializer.SerializeToBytes(
            new
            {
                state = lighting.State.ToString().ToUpperInvariant(),
                brightness = lighting.Level == null ? default(double?) : lighting.Level.Value,
                color_temp = lighting.Temperature == null ? default(double?) : lighting.Temperature.Value,
                color = lighting.Color == null
                    ? null
                    : new
                    {
                        b = lighting.Color.Blue,
                        g = lighting.Color.Green,
                        r = lighting.Color.Red,
                    },
            }
        );
    }
}
