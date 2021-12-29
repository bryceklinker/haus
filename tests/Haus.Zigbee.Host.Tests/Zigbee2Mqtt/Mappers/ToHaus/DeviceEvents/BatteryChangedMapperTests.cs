using FluentAssertions;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;

public class BatteryChangedMapperTests
{
    private readonly BatteryChangedMapper _mapper;

    public BatteryChangedMapperTests()
    {
        _mapper = new BatteryChangedMapper();
    }

    [Fact]
    public void WhenBatteryChangedThenReturnsPopulatedBatteryChanged()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithBatteryLevel(43)
            .WithDeviceTopic("my-device-id")
            .BuildZigbee2MqttMessage();

        var model = _mapper.Map(message);

        model.BatteryLevel.Should().Be(43);
        model.DeviceId.Should().Be("my-device-id");
    }

    [Fact]
    public void WhenBatteryLevelNotReportedThenReturnsNull()
    {
        var message = new Zigbee2MqttMessageBuilder().BuildZigbee2MqttMessage();

        _mapper.Map(message).Should().BeNull();
    }
}