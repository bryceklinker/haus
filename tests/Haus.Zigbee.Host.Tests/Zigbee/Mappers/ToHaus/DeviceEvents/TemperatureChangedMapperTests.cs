using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus.DeviceEvents;

public class TemperatureChangedMapperTests
{
    private readonly TemperatureChangedMapper _mapper = new();

    [Fact]
    public void Map_DividesCentidegreesDownToDegrees()
    {
        var value = new ZclAttributeValue(ZclDataType.Int16, 6500);

        var model = _mapper.Map("1234", value);

        Assert.Equal("1234", model.DeviceId);
        Assert.Equal(65.0, model.Temperature);
    }

    [Fact]
    public void Map_NegativeValue_ReturnsNegativeDegrees()
    {
        var value = new ZclAttributeValue(ZclDataType.Int16, unchecked((ulong)(ushort)-350));

        var model = _mapper.Map("1234", value);

        Assert.Equal(-3.5, model.Temperature);
    }
}
