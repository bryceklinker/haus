using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;
using Newtonsoft.Json.Linq;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus
{
    public class DevicesMapper : IToHausMapper
    {
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
        private readonly IOptions<HausOptions> _hausOptions;
        private readonly IDeviceTypeResolver _deviceTypeResolver;

        public DevicesMapper(
            IOptions<ZigbeeOptions> zigbeeOptions,
            IOptions<HausOptions> hausOptions,
            IDeviceTypeResolver deviceTypeResolver)
        {
            _zigbeeOptions = zigbeeOptions;
            _hausOptions = hausOptions;
            _deviceTypeResolver = deviceTypeResolver;
        }

        public bool IsSupported(Zigbee2MqttMessage message)
        {
            return message.Topic == $"{_zigbeeOptions.GetBaseTopic()}/bridge/config/devices";
        }

        public IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage message)
        {
            return message.PayloadArray
                .Cast<JObject>()
                .Select(item => new MqttApplicationMessage
                {
                    Topic = _hausOptions.GetEventsTopic(),
                    Payload = HausJsonSerializer.SerializeToBytes(CreateDeviceDiscoveredEvent(item))
                });
        }

        private HausEvent<DeviceDiscoveredModel> CreateDeviceDiscoveredEvent(JObject jToken)
        {
            var model = jToken.Value<string>("model");
            var vendor = jToken.Value<string>("vendor");
            return new DeviceDiscoveredModel(jToken.Value<string>("friendly_name"),
                _deviceTypeResolver.Resolve(vendor, model),
                CreateDeviceMetadata(jToken)
            ).AsHausEvent();
        }

        private static MetadataModel[] CreateDeviceMetadata(JObject jObject)
        {
            return jObject.ToDeviceMetadata()
                .ToArray();
        }
    }
}