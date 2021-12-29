using System.Linq;
using FluentAssertions;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus;

public class ZigbeeToHausModelMapperTests
{
    private const string HausEventTopic = ConfigurationFactory.DefaultHausEventsTopic;
    private const string UnknownEventTopic = ConfigurationFactory.DefaultHausUnknownTopic;
    private readonly ZigbeeToHausMapper _mapper;

    public ZigbeeToHausModelMapperTests()
    {
        var provider = ServiceProviderFactory.Create();
        var mappers = provider.GetServices<IToHausMapper>();
        var factory = provider.GetRequiredService<IZigbee2MqttMessageFactory>();
        var unknownMapper = provider.GetRequiredService<IUnknownMessageMapper>();

        _mapper = new ZigbeeToHausMapper(factory, mappers, unknownMapper);
    }

    [Fact]
    public void WhenInterviewSuccessfulMessageThenReturnsDeviceDiscovered()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithLogTopic()
            .WithInterviewSuccessful()
            .WithPairingType()
            .WithMeta(meta => meta.WithFriendlyName("this-is-an-id"))
            .BuildMqttMessage();

        var result = _mapper.Map(message).Single();

        result.Topic.Should().Be(HausEventTopic);
    }

    [Fact]
    public void WhenStateMessageThenReturnsUnknownMessage()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithStateTopic()
            .WithState("online")
            .BuildMqttMessage();

        var result = _mapper.Map(message).Single();

        result.Topic.Should().Be(UnknownEventTopic);
    }

    [Fact]
    public void WhenFromSensorThenReturnsHausEvent()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithDeviceTopic("some-device-name")
            .WithIlluminance(4)
            .BuildMqttMessage();

        var result = _mapper.Map(message).Single();

        result.Topic.Should().Be(HausEventTopic);
    }
}