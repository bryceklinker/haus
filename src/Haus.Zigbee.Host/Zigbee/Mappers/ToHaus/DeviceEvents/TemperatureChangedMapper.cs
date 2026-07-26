using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;

public class TemperatureChangedMapper
{
    private const double CentidegreesPerDegree = 100.0;

    public TemperatureChangedModel Map(string deviceId, ZclAttributeValue value)
    {
        return new TemperatureChangedModel(deviceId, value.AsSigned() / CentidegreesPerDegree);
    }
}
