using System;
using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Sensors;
using Haus.Core.Models.Sensors.Battery;
using Haus.Core.Models.Sensors.Light;
using Haus.Core.Models.Sensors.Motion;
using Haus.Core.Models.Sensors.Temperature;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices
{
    public class DeviceMessageMapper : MapperBase
    {
        private readonly SensorChangedMapper _sensorChangedMapper;
        
        public DeviceMessageMapper(IOptions<HausOptions> options)
            : base(options)
        {
            _sensorChangedMapper = new SensorChangedMapper();
        }

        public override MqttApplicationMessage Map(Zigbee2MqttMessage message)
        {
            return new MqttApplicationMessage
            {
                Topic = EventTopicName,
                Payload = MapMessageToPayload(message)
            };
        }

        private byte[] MapMessageToPayload(Zigbee2MqttMessage message)
        {
            var payload = _sensorChangedMapper.Map(message);
            return JsonSerializer.SerializeToUtf8Bytes(new HausEvent
            {
                Type = GetHausEventType(payload),
                Payload = JsonSerializer.Serialize(payload)
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
                _ => throw new InvalidOperationException($"Unknown payload type: {payload?.GetType()}")
            };
        }
    }
}