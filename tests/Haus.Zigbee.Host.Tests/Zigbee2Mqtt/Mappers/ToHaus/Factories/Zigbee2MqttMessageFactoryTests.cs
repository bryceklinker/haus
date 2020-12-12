using System.Text;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.Factories
{
    public class Zigbee2MqttMessageFactoryTests
    {
        private readonly Zigbee2MqttMessageFactory _factory;

        public Zigbee2MqttMessageFactoryTests()
        {
            _factory = new Zigbee2MqttMessageFactory(new NullLogger<Zigbee2MqttMessageFactory>());
        }

        [Fact]
        public void WhenPayloadContainsAJsonObjectThenReturnsMessageWithRootPropertiesAvailable()
        {
            var bytes = Encoding.UTF8.GetBytes("{\"id\": 54}");

            var message = _factory.Create(new MqttApplicationMessage {Payload = bytes});

            Assert.Equal(54, message.PayloadObject.Value<int>("id"));
        }

        [Fact]
        public void WhenPayloadContainsJsonArrayThenReturnsMessageWithArrayAvailable()
        {
            var bytes = Encoding.UTF8.GetBytes("[{}, {}, {}]");

            var message = _factory.Create(new MqttApplicationMessage {Payload = bytes});

            Assert.Equal(3, message.PayloadArray.Count);
        }
    }
}