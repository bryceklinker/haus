using System.Text;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee
{
    public interface IHausToZigbeeMapper
    {
        MqttApplicationMessage Map(MqttApplicationMessage message);
    }
    
    public class HausToZigbeeMapper : IHausToZigbeeMapper
    {
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;

        private string Zigbee2MqttBaseTopic => _zigbeeOptions.Value.Config.Mqtt.BaseTopic;
        public HausToZigbeeMapper(IOptions<ZigbeeOptions> zigbeeOptions)
        {
            _zigbeeOptions = zigbeeOptions;
        }

        public MqttApplicationMessage Map(MqttApplicationMessage message)
        {
            var command = HausJsonSerializer.Deserialize<HausCommand>(message.Payload);
            if (command.Type == StartDiscoveryModel.Type)
                return CreatePermitJoinMessage();
            
            return null;
        }

        private MqttApplicationMessage CreatePermitJoinMessage()
        {
            return new MqttApplicationMessage
            {
                Topic = $"{Zigbee2MqttBaseTopic}/bridge/config/permit_join",
                Payload = Encoding.UTF8.GetBytes("true")
            };
        }
    }
}