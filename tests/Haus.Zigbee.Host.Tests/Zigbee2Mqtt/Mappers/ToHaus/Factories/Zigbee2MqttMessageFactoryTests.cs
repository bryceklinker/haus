using System.Text;
using FluentAssertions;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.Factories;

public class Zigbee2MqttMessageFactoryTests
{
    private readonly Zigbee2MqttMessageFactory _factory = new(new NullLogger<Zigbee2MqttMessageFactory>());

    [Fact]
    public void WhenPayloadIsNullThenReturnsMessageWithNullValue()
    {
        var message = _factory.Create(new MqttApplicationMessage { PayloadSegment = null });

        message.PayloadObject.Should().BeNull();
    }

    [Fact]
    public void WhenPayloadContainsAJsonObjectThenReturnsMessageWithRootPropertiesAvailable()
    {
        var bytes = Encoding.UTF8.GetBytes("{\"id\": 54}");

        var message = _factory.Create(new MqttApplicationMessage { PayloadSegment = bytes });

        message.PayloadObject.Value<int>("id").Should().Be(54);
    }

    [Fact]
    public void WhenPayloadContainsJsonArrayThenReturnsMessageWithArrayAvailable()
    {
        var bytes = Encoding.UTF8.GetBytes("[{}, {}, {}]");

        var message = _factory.Create(new MqttApplicationMessage { PayloadSegment = bytes });

        message.PayloadArray.Should().HaveCount(3);
    }
}
