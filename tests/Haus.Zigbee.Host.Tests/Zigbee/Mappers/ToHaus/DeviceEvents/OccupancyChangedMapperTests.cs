using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus.DeviceEvents;

public class OccupancyChangedMapperTests
{
    private readonly OccupancyChangedMapper _mapper = new();

    [Fact]
    public void Map_OccupiedBitSet_ReturnsOccupancyTrue()
    {
        var value = new ZclAttributeValue(ZclDataType.Bitmap8, 0x01);

        var model = _mapper.Map("motions", value);

        Assert.Equal("motions", model.DeviceId);
        Assert.True(model.Occupancy);
    }

    [Fact]
    public void Map_OccupiedBitClear_ReturnsOccupancyFalse()
    {
        var value = new ZclAttributeValue(ZclDataType.Bitmap8, 0x00);

        var model = _mapper.Map("motions", value);

        Assert.False(model.Occupancy);
    }

    [Fact]
    public void Map_OnlyOtherBitsSet_ReturnsOccupancyFalse()
    {
        var value = new ZclAttributeValue(ZclDataType.Bitmap8, 0x02);

        var model = _mapper.Map("motions", value);

        Assert.False(model.Occupancy);
    }
}
