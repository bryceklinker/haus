using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;

public class OccupancyChangedMapper
{
    private const ulong OccupiedBit = 0x01;

    public OccupancyChangedModel Map(string deviceId, ZclAttributeValue value)
    {
        return new OccupancyChangedModel(deviceId, (value.AsUnsigned() & OccupiedBit) != 0);
    }
}
