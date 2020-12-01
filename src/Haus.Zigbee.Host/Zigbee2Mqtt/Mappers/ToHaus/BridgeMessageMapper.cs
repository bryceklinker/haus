using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Pairing;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus
{
    public class BridgeMessageMapper : IMapper
    {
        private readonly PairingMessageMapper _pairingMessageMapper;
        private readonly UnknownMessageMapper _unknownMessageMapper;

        public BridgeMessageMapper(IOptionsMonitor<HausOptions> hausOptions)
        {
            _pairingMessageMapper = new PairingMessageMapper(hausOptions);
            _unknownMessageMapper = new UnknownMessageMapper(hausOptions);
        }

        public MqttApplicationMessage Map(Zigbee2MqttMessage zigbeeMessage)
        {
            if (zigbeeMessage.Type == "pairing")
                return _pairingMessageMapper.Map(zigbeeMessage);

            return _unknownMessageMapper.Map(zigbeeMessage);
        }
    }
}