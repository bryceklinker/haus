using FluentAssertions;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;

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

        model?.DeviceId.Should().Be("1234");
        model?.Temperature.Should().Be(65);
    }

    [Fact]
    public void WhenTemperatureNotReportedThenReturnsNull()
    {
        var message = new Zigbee2MqttMessageBuilder().BuildZigbee2MqttMessage();

        _mapper.Map(message).Should().BeNull();
    }
}
