using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;

public class BatteryChangedMapper
{
    private const double HalfPercentUnitsPerPercent = 2.0;

    public BatteryChangedModel Map(string deviceId, ZclAttributeValue value)
    {
        var percentage = value.AsUnsigned() / HalfPercentUnitsPerPercent;
        return new BatteryChangedModel(deviceId, (long)percentage);
    }
}
