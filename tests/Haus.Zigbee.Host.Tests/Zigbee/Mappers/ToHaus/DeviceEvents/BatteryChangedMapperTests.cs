using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus.DeviceEvents;

public class BatteryChangedMapperTests
{
    private readonly BatteryChangedMapper _mapper = new();

    [Fact]
    public void Map_HalvesThePercentageUnitsFromThePowerConfigCluster()
    {
        var value = new ZclAttributeValue(ZclDataType.Uint8, 86);

        var model = _mapper.Map("my-device-id", value);

        Assert.Equal("my-device-id", model.DeviceId);
        Assert.Equal(43, model.BatteryLevel);
    }
}
