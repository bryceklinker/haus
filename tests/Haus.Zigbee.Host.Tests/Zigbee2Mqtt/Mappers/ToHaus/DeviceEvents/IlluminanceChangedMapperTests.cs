using FluentAssertions;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;

public class IlluminanceChangedMapperTests
{
    private readonly IlluminanceChangedMapper _mapper = new();

    [Fact]
    public void WhenIlluminanceChangedThenReturnsPopulatedIlluminanceChanged()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithDeviceTopic("1231")
            .WithIlluminance(65)
            .WithIlluminanceLux(12)
            .BuildZigbee2MqttMessage();

        var model = _mapper.Map(message);

        model?.Illuminance.Should().Be(65);
        model?.Lux.Should().Be(12);
        model?.DeviceId.Should().Be("1231");
    }

    [Fact]
    public void WhenIlluminanceNotReportedThenReturnsNull()
    {
        var message = new Zigbee2MqttMessageBuilder().BuildZigbee2MqttMessage();

        _mapper.Map(message).Should().BeNull();
    }
}
