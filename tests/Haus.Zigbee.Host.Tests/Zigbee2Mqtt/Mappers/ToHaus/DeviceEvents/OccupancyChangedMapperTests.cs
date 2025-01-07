using FluentAssertions;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;

public class OccupancyChangedMapperTests
{
    private readonly OccupancyChangedMapper _mapper = new();

    [Fact]
    public void WhenOccupancyChangedThenReturnsPopulatedOccupancyChanged()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithDeviceTopic("motions")
            .WithOccupancy(true)
            .WithOccupancyTimeout(123)
            .WithMotionSensitivity("low")
            .BuildZigbee2MqttMessage();

        var model = _mapper.Map(message);

        model.DeviceId.Should().Be("motions");
        model.Occupancy.Should().BeTrue();
        model.Timeout.Should().Be(123);
        model.Sensitivity.Should().Be("low");
    }

    [Fact]
    public void WhenOccupancyNotReportedThenReturnsNull()
    {
        var message = new Zigbee2MqttMessageBuilder().BuildZigbee2MqttMessage();

        _mapper.Map(message).Should().BeNull();
    }
}