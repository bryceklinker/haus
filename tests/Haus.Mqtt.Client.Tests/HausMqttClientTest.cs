using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Mqtt.Client.Settings;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.Options;
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
            var options = Options.Create(new HausMqttSettings());
            _client = new HausMqttClient(_fakeMqttClient, options);
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

            actual.Should().BeSameAs(expected);
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
            actuals.Should().HaveCount(2);
        }
    }
}