using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;

public class InterviewSuccessfulMapper : IToHausMapper
{
    private readonly IOptions<HausOptions> _hausOptions;
    private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
    private readonly IDeviceTypeResolver _deviceTypeResolver;

    private string EventsTopic => _hausOptions.Value.EventsTopic;

    public InterviewSuccessfulMapper(IOptions<HausOptions> hausOptions, IOptions<ZigbeeOptions> zigbeeOptions)
    {
        _hausOptions = hausOptions;
        _zigbeeOptions = zigbeeOptions;
        _deviceTypeResolver = new DeviceTypeResolver(_hausOptions);
    }

    public bool IsSupported(Zigbee2MqttMessage message)
    {
        return message.Topic == $"{_zigbeeOptions.GetBaseTopic()}/bridge/log"
            && message is { Message: "interview_successful", Type: "pairing" };
    }

    public IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage zigbeeMessage)
    {
        var id = zigbeeMessage.Meta?.FriendlyName ?? "";
        var meta = zigbeeMessage.Meta;
        var deviceType = meta == null ? DeviceType.Unknown : _deviceTypeResolver.Resolve(meta);
        var metadata = meta?.Root.ToDeviceMetadata() ?? [];
        var payload = new DeviceDiscoveredEvent(id, deviceType, metadata.ToArray());
        yield return new MqttApplicationMessage
        {
            Topic = EventsTopic,
            PayloadSegment = HausJsonSerializer.SerializeToBytes(
                new HausEvent<DeviceDiscoveredEvent>(DeviceDiscoveredEvent.Type, payload)
            ),
        };
    }
}
