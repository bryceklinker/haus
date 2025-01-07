using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.ExternalMessages;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;

public interface IHausToZigbeeMapper
{
    IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message);
}

public class HausToZigbeeMapper(IEnumerable<IToZigbeeMapper> mappers) : IHausToZigbeeMapper
{
    public IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message)
    {
        var command = HausJsonSerializer.Deserialize<HausCommand>(message.PayloadSegment);
        return mappers.Where(m => m.IsSupported(command.Type))
            .SelectMany(m => m.Map(message));
    }
}