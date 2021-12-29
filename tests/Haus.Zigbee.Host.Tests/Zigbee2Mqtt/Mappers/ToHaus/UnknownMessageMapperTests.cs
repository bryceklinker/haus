using System.Linq;
using FluentAssertions;
using Haus.Core.Models;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus;

public class UnknownMessageMapperTests
{
    private const string UnknownTopicName = "idk";
    private readonly UnknownMessageMapper _mapper;

    public UnknownMessageMapperTests()
    {
        var options = new OptionsMonitorFake<HausOptions>(new HausOptions
        {
            UnknownTopic = UnknownTopicName
        });
        _mapper = new UnknownMessageMapper(options);
    }

    [Fact]
    public void WhenIsSupportedThenAlwaysReturnsFalse()
    {
        var message = Zigbee2MqttMessage.FromJToken("", JObject.FromObject(new object()));
        _mapper.IsSupported(message).Should().BeFalse("Message was supported");
    }

    [Fact]
    public void WhenMappedThenTopicIsUnknownTopic()
    {
        var message = Zigbee2MqttMessage.FromJToken("", JObject.FromObject(new object()));

        var result = _mapper.Map(message).Single();

        result.Topic.Should().Be(UnknownTopicName);
    }

    [Fact]
    public void WhenMappedThenZigbeeTopicIsInMessagePayload()
    {
        var message = Zigbee2MqttMessage.FromJToken("zigbeetopic", JObject.FromObject(new object()));

        var result = _mapper.Map(message).Single();

        var payload = HausJsonSerializer.Deserialize<UnknownModel>(result.Payload);
        payload.Topic.Should().Be("zigbeetopic");
    }

    [Fact]
    public void WhenMappedThenZigbeePayloadIsInMessagePayload()
    {
        var message = Zigbee2MqttMessage.FromJToken("", JObject.FromObject(new
        {
            Id = "my-id"
        }));

        var result = _mapper.Map(message).Single();

        var payload = HausJsonSerializer.Deserialize<UnknownModel>(result.Payload);
        JObject.Parse(payload.Payload).Value<string>("Id").Should().Be("my-id");
    }
}