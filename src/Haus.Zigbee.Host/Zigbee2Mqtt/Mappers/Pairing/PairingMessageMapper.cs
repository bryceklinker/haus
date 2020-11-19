using System;
using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Discovery;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Pairing
{
    public class DeviceDiscoveredMapper
    {
        private readonly IOptions<HausOptions> _hausOptions;

        private string EventsTopic => _hausOptions.Value.EventsTopic;

        public DeviceDiscoveredMapper(IOptions<HausOptions> hausOptions)
        {
            _hausOptions = hausOptions;
        }

        public MqttApplicationMessage Map(Zigbee2MqttMessage zigbeeMessage)
        {
            return new MqttApplicationMessage
            {
                Topic = EventsTopic,
                Payload = JsonSerializer.SerializeToUtf8Bytes(new HausEvent
                {
                    Type = DeviceDiscoveredModel.Type,
                    Payload = JsonSerializer.Serialize(new DeviceDiscoveredModel
                    {
                        Id = zigbeeMessage.Meta.FriendlyName,
                        Description = zigbeeMessage.Meta.Description,
                        Model = zigbeeMessage.Meta.Model,
                        Vendor = zigbeeMessage.Meta.Vendor
                    })
                })
            };
        }
    }
    
    public class PairingMessageMapper
    {
        private readonly DeviceDiscoveredMapper _deviceDiscoveredMapper;
        private readonly UnknownMessageMapper _unknownMessageMapper;

        public PairingMessageMapper(IOptions<HausOptions> hausOptions)
        {
            _deviceDiscoveredMapper = new DeviceDiscoveredMapper(hausOptions);
            _unknownMessageMapper = new UnknownMessageMapper(hausOptions);
        }

        public MqttApplicationMessage Map(Zigbee2MqttMessage zigbeeMessage)
        {
            if (zigbeeMessage.Message == "interview_successful")
                return _deviceDiscoveredMapper.Map(zigbeeMessage);

            return _unknownMessageMapper.Map(zigbeeMessage);
        }
    }
}