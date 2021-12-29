using Haus.Core.Models.Devices.Sensors;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;

public class SensorChangedMapper
{
    private readonly BatteryChangedMapper _batteryChangedMapper;
    private readonly IlluminanceChangedMapper _illuminanceChangedMapper;
    private readonly OccupancyChangedMapper _occupancyChangedMapper;
    private readonly TemperatureChangedMapper _temperatureChangedMapper;

    public SensorChangedMapper()
    {
        _batteryChangedMapper = new BatteryChangedMapper();
        _illuminanceChangedMapper = new IlluminanceChangedMapper();
        _occupancyChangedMapper = new OccupancyChangedMapper();
        _temperatureChangedMapper = new TemperatureChangedMapper();
    }

    public object Map(Zigbee2MqttMessage message)
    {
        var multiSensorChanged = new MultiSensorChanged(
            message.GetFriendlyNameFromTopic(),
            _occupancyChangedMapper.Map(message),
            _temperatureChangedMapper.Map(message),
            _illuminanceChangedMapper.Map(message),
            _batteryChangedMapper.Map(message)
        );

        return multiSensorChanged.HasMultipleChanges
            ? multiSensorChanged
            : multiSensorChanged.GetSingleChange();
    }
}