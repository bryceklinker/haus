using System.Collections.Generic;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;

public class DeviceEventMapper : ToHausMapperBase
{
    private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
    private readonly ILogger<DeviceEventMapper> _logger;
    private readonly SensorChangedMapper _sensorChangedMapper;

    public DeviceEventMapper(IOptionsMonitor<HausOptions> options, IOptions<ZigbeeOptions> zigbeeOptions,
        ILogger<DeviceEventMapper> logger)
        : base(options)
    {
        _zigbeeOptions = zigbeeOptions;
        _logger = logger;
        _sensorChangedMapper = new SensorChangedMapper();
    }

    public override bool IsSupported(Zigbee2MqttMessage message)
    {
        return message.Topic.StartsWith(_zigbeeOptions.GetBaseTopic())
               && message.Topic.Split('/').Length == 2;
    }

    public override IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage message)
    {
        yield return new MqttApplicationMessage
        {
            Topic = EventTopicName,
            Payload = MapMessageToPayload(message)
        };
    }

    private byte[] MapMessageToPayload(Zigbee2MqttMessage message)
    {
        var payload = _sensorChangedMapper.Map(message);
        var type = GetHausEventType(payload);
        if (type == UnknownEvent.Type) _logger.LogWarning("Unknown payload received: {@Payload}", payload);

        return HausJsonSerializer.SerializeToBytes(new HausEvent<object>
        {
            Type = type,
            Payload = payload
        });
    }

    private string GetHausEventType(object payload)
    {
        return payload switch
        {
            MultiSensorChanged => MultiSensorChanged.Type,
            IlluminanceChangedModel => IlluminanceChangedModel.Type,
            BatteryChangedModel => BatteryChangedModel.Type,
            OccupancyChangedModel => OccupancyChangedModel.Type,
            TemperatureChangedModel => TemperatureChangedModel.Type,
            _ => UnknownEvent.Type
        };
    }
}