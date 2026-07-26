using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus.DeviceEvents;

public class TemperatureChangedMapperTests
{
    private readonly TemperatureChangedMapper _mapper = new();

    [Fact]
    public void WhenTemperatureChangedThenReturnsPopulatedTemperatureChanged()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithTemperature(65)
            .WithDeviceTopic("1234")
            .BuildZigbee2MqttMessage();

        var model = _mapper.Map(message);

        Assert.Equal("1234", model?.DeviceId);
        Assert.Equal(65, model?.Temperature);
    }

    [Fact]
    public void WhenTemperatureNotReportedThenReturnsNull()
    {
        var message = new Zigbee2MqttMessageBuilder().BuildZigbee2MqttMessage();

        Assert.Null(_mapper.Map(message));
    }
}
