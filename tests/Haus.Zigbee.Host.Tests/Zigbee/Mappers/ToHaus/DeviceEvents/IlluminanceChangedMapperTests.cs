using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus.DeviceEvents;

public class IlluminanceChangedMapperTests
{
    private readonly IlluminanceChangedMapper _mapper = new();

    [Fact]
    public void Map_ComputesLuxFromTheZclMeasuredValueFormula()
    {
        var value = new ZclAttributeValue(ZclDataType.Uint16, 10001);

        var model = _mapper.Map("1231", value);

        Assert.Equal("1231", model.DeviceId);
        Assert.Equal(10001, model.Illuminance);
        Assert.Equal(10, model.Lux);
    }

    [Fact]
    public void Map_TooLowToBeMeasuredValue_ReturnsNullLux()
    {
        var value = new ZclAttributeValue(ZclDataType.Uint16, 0);

        var model = _mapper.Map("1231", value);

        Assert.Null(model.Lux);
    }

    [Fact]
    public void Map_InvalidValue_ReturnsNullLux()
    {
        var value = new ZclAttributeValue(ZclDataType.Uint16, 0xffff);

        var model = _mapper.Map("1231", value);

        Assert.Null(model.Lux);
    }
}
