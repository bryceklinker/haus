using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;

public class DevicesMapper(
    IOptions<ZigbeeOptions> zigbeeOptions,
    IOptions<HausOptions> hausOptions,
    IDeviceTypeResolver deviceTypeResolver
) : IToHausMapper
{
    public bool IsSupported(Zigbee2MqttMessage message)
    {
        return message.Topic == $"{zigbeeOptions.GetBaseTopic()}/bridge/config/devices";
    }

    public IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage message)
    {
        return message
                .PayloadArray?.Cast<JObject>()
                .Select(item => new MqttApplicationMessage
                {
                    Topic = hausOptions.GetEventsTopic(),
                    PayloadSegment = HausJsonSerializer.SerializeToBytes(CreateDeviceDiscoveredEvent(item)),
                }) ?? [];
    }

    private HausEvent<DeviceDiscoveredEvent> CreateDeviceDiscoveredEvent(JObject jToken)
    {
        var model = jToken.Value<string>("model") ?? "";
        var vendor = jToken.Value<string>("vendor") ?? "";
        var id = jToken.Value<string>("friendly_name") ?? "";
        return new DeviceDiscoveredEvent(
            id,
            deviceTypeResolver.Resolve(vendor, model),
            CreateDeviceMetadata(jToken)
        ).AsHausEvent();
    }

    private static MetadataModel[] CreateDeviceMetadata(JObject jObject)
    {
        return jObject.ToDeviceMetadata().ToArray();
    }
}
