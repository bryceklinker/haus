using System.Collections.Generic;
using System.Threading.Tasks;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using MQTTnet.Client;
using Xunit;

namespace Haus.Mqtt.Client.Tests
{
    public class HausMqttClientTest
    {
        private readonly HausMqttClient _client;
        private readonly FakeMqttClient _fakeMqttClient;

        public HausMqttClientTest()
        {
            _fakeMqttClient = new FakeMqttClient();
            _client = new HausMqttClient(_fakeMqttClient, new NullLoggerFactory());
        }

        [Fact]
        public async Task WhenSubscribingToTopicThenPublishedMessageGoesToSubscriber()
        {
            MqttApplicationMessage actual = null;
            await _client.SubscribeAsync("bob", msg =>
            {
                actual = msg;
                return Task.CompletedTask;
            });
            
            var expected = new MqttApplicationMessage{Topic = "bob"};
            await _fakeMqttClient.PublishAsync(expected);

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task WhenMultipleSubscribersThenPublishedMessagesGoToAllSubscribers()
        {
            var actuals = new List<MqttApplicationMessage>();
            await _client.SubscribeAsync("#", msg =>
            {
                actuals.Add(msg);
                return Task.CompletedTask;
            });
            await _client.SubscribeAsync("#", msg =>
            {
                actuals.Add(msg);
                return Task.CompletedTask;
            });

            await _fakeMqttClient.PublishAsync(new MqttApplicationMessage());
            Assert.Equal(2, actuals.Count);
        }
    }
}