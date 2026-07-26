using System.Linq;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus;

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
    public void WhenInterviewSuccessfulMessageThenReturnsUnknownMessage()
    {
        // Device-joined handling moved off this MQTT-relay aggregator onto DeviceJoinedMapper,
        // wired directly into the coordinator-driven relay -- nothing here claims this message.
        var message = new Zigbee2MqttMessageBuilder()
            .WithLogTopic()
            .WithInterviewSuccessful()
            .WithPairingType()
            .WithMeta(meta => meta.WithFriendlyName("this-is-an-id"))
            .BuildMqttMessage();

        var result = _mapper.Map(message).Single();

        Assert.Equal(UnknownEventTopic, result.Topic);
    }

    [Fact]
    public void WhenStateMessageThenReturnsUnknownMessage()
    {
        var message = new Zigbee2MqttMessageBuilder().WithStateTopic().WithState("online").BuildMqttMessage();

        var result = _mapper.Map(message).Single();

        Assert.Equal(UnknownEventTopic, result.Topic);
    }

    [Fact]
    public void WhenFromSensorThenReturnsUnknownMessage()
    {
        // Sensor-attribute handling moved off this MQTT-relay aggregator onto DeviceEventMapper,
        // wired directly into the coordinator-driven relay -- nothing here claims this message.
        var message = new Zigbee2MqttMessageBuilder()
            .WithDeviceTopic("some-device-name")
            .WithIlluminance(4)
            .BuildMqttMessage();

        var result = _mapper.Map(message).Single();

        Assert.Equal(UnknownEventTopic, result.Topic);
    }
}
