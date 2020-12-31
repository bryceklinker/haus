using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus
{
    public class InterviewSuccessfulMapper : IToHausMapper
    {
        private readonly IOptions<HausOptions> _hausOptions;
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
        private readonly IDeviceTypeResolver _deviceTypeResolver;

        private string EventsTopic => _hausOptions.Value.EventsTopic;

        public InterviewSuccessfulMapper(
            IOptions<HausOptions> hausOptions,
            IOptions<ZigbeeOptions> zigbeeOptions)
        {
            _hausOptions = hausOptions;
            _zigbeeOptions = zigbeeOptions;
            _deviceTypeResolver = new DeviceTypeResolver(_hausOptions);
        }

        public bool IsSupported(Zigbee2MqttMessage message)
        {
            return message.Topic == $"{_zigbeeOptions.GetBaseTopic()}/bridge/log"
                   && message.Message == "interview_successful"
                   && message.Type == "pairing";
        }

        public IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage zigbeeMessage)
        {
            yield return new MqttApplicationMessage
            {
                Topic = EventsTopic,
                Payload = HausJsonSerializer.SerializeToBytes(new HausEvent<DeviceDiscoveredModel>
                {
                    Type = DeviceDiscoveredModel.Type,
                    Payload = new DeviceDiscoveredModel(
                        zigbeeMessage.Meta.FriendlyName,
                        _deviceTypeResolver.Resolve(zigbeeMessage.Meta),
                        zigbeeMessage.Meta.Root.ToDeviceMetadata().ToArray()
                    )
                })
            };
        }
    }
}