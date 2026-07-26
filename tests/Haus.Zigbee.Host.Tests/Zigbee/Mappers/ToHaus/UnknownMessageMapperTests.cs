using System;
using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus;

public class UnknownMessageMapperTests
{
    private const string UnknownTopicName = "idk";
    private readonly UnknownMessageMapper _mapper;

    public UnknownMessageMapperTests()
    {
        var options = new OptionsMonitorFake<HausOptions>(new HausOptions { UnknownTopic = UnknownTopicName });
        _mapper = new UnknownMessageMapper(options);
    }

    [Fact]
    public void WhenIsSupportedThenAlwaysReturnsFalse()
    {
        var message = Zigbee2MqttMessage.FromJToken("", JObject.FromObject(new object()));
        Assert.False(_mapper.IsSupported(message), "Message was supported");
    }

    [Fact]
    public void WhenMappedThenTopicIsUnknownTopic()
    {
        var message = Zigbee2MqttMessage.FromJToken("", JObject.FromObject(new object()));

        var result = _mapper.Map(message).Single();

        Assert.Equal(UnknownTopicName, result.Topic);
    }

    [Fact]
    public void WhenMappedThenZigbeeTopicIsInMessagePayload()
    {
        var message = Zigbee2MqttMessage.FromJToken("zigbeetopic", JObject.FromObject(new object()));

        var result = _mapper.Map(message).Single();

        var payload = HausJsonSerializer.Deserialize<UnknownModel>(result.PayloadSegment);
        Assert.Equal("zigbeetopic", payload?.Topic);
    }

    [Fact]
    public void WhenMappedThenZigbeePayloadIsInMessagePayload()
    {
        var message = Zigbee2MqttMessage.FromJToken("", JObject.FromObject(new { Id = "my-id" }));

        var result = _mapper.Map(message).Single();

        var payload = HausJsonSerializer.Deserialize<UnknownModel>(result.PayloadSegment);
        ArgumentNullException.ThrowIfNull(payload);
        Assert.Equal("my-id", JObject.Parse(payload.Payload).Value<string>("Id"));
    }
}
