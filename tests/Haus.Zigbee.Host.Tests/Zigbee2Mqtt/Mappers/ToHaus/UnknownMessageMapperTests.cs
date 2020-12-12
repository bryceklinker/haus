using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus
{
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
            Assert.False(_mapper.IsSupported(Zigbee2MqttMessage.FromJToken("", JObject.FromObject(new object()))));
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

            var payload = HausJsonSerializer.Deserialize<UnknownModel>(result.Payload);
            Assert.Equal("zigbeetopic", payload.Topic);
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
            Assert.Equal("my-id", JObject.Parse(payload.Payload).Value<string>("Id"));
        }
    }
}