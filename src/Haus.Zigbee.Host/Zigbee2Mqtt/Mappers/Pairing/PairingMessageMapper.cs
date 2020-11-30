using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Pairing
{
    public class PairingMessageMapper
    {
        private readonly DeviceDiscoveredMapper _deviceDiscoveredMapper;
        private readonly UnknownMessageMapper _unknownMessageMapper;

        public PairingMessageMapper(IOptionsMonitor<HausOptions> hausOptionsMonitor)
        {
            _deviceDiscoveredMapper = new DeviceDiscoveredMapper(hausOptionsMonitor);
            _unknownMessageMapper = new UnknownMessageMapper(hausOptionsMonitor);
        }

        public MqttApplicationMessage Map(Zigbee2MqttMessage zigbeeMessage)
        {
            if (zigbeeMessage.Message == "interview_successful")
                return _deviceDiscoveredMapper.Map(zigbeeMessage);

            return _unknownMessageMapper.Map(zigbeeMessage);
        }
    }
}