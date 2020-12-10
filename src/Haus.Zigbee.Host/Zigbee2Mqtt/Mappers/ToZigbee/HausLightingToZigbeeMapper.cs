using System.Collections.Generic;
using Haus.Core.Models;
using Haus.Core.Models.Common;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee
{
    public interface IHausLightingToZigbeeMapper
    {
        byte[] Map(LightingModel lighting);
    }

    public class HausLightingToZigbeeMapper : IHausLightingToZigbeeMapper
    {
        public byte[] Map(LightingModel lighting)
        {
            return HausJsonSerializer.SerializeToBytes(new
            {
                state = lighting.State.ToString().ToUpperInvariant(),
                brightness = lighting.Brightness,
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