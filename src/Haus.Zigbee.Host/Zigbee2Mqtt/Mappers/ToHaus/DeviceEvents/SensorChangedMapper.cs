using Haus.Core.Models.Devices.Sensors;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;

public class SensorChangedMapper
{
    private readonly BatteryChangedMapper _batteryChangedMapper = new();
    private readonly IlluminanceChangedMapper _illuminanceChangedMapper = new();
    private readonly OccupancyChangedMapper _occupancyChangedMapper = new();
    private readonly TemperatureChangedMapper _temperatureChangedMapper = new();

    public object Map(Zigbee2MqttMessage message)
    {
        var multiSensorChanged = new MultiSensorChanged(
            message.GetFriendlyNameFromTopic(),
            _occupancyChangedMapper.Map(message),
            _temperatureChangedMapper.Map(message),
            _illuminanceChangedMapper.Map(message),
            _batteryChangedMapper.Map(message)
        );

        return multiSensorChanged.HasMultipleChanges ? multiSensorChanged : multiSensorChanged.GetSingleChange();
    }
}
