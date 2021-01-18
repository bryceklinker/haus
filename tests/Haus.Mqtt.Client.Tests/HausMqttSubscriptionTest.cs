using System.Threading.Tasks;
using FluentAssertions;
using Haus.Mqtt.Client.Settings;
using Haus.Mqtt.Client.Subscriptions;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Diagnostics;
using Xunit;

namespace Haus.Mqtt.Client.Tests
{
    public class HausMqttSubscriptionTest : IAsyncLifetime
    {
        private IHausMqttClient _client;

        public async Task InitializeAsync()
        {
            var options = Options.Create(new HausMqttSettings
            {
                Server = "mqtt://localhost"
            });

            var factory = new HausMqttClientFactory(options, new FakeMqttClientFactory(), new MqttNetLogger());
            _client = await factory.CreateClient();
        }

        [Fact]
        public async Task WhenExecutedForMessageWithDifferentTopicThenSubscriberIsNotExecuted()
        {
            MqttApplicationMessage actual = null;
            await _client.SubscribeAsync("one", msg =>
            {
                actual = msg;
                return Task.CompletedTask;
            });

            await _client.PublishAsync(new MqttApplicationMessage {Topic = "other"});

            actual.Should().BeNull();
        }

        [Fact]
        public async Task WhenSubscribedToAllTopicsThenExecuteAlwaysInvokesSubscriber()
        {
            MqttApplicationMessage actual = null;
            await _client.SubscribeAsync("#", msg =>
            {
                actual = msg;
                return Task.CompletedTask;
            });
            
            var expected = new MqttApplicationMessage{Topic = "other"};
            await _client.PublishAsync(expected);

            Eventually.Assert(() =>
            {
                actual.Should().BeEquivalentTo(expected);
            });
        }

        [Fact]
        public async Task WhenUnsubscribedThenNoLongerReceivesMessages()
        {
            var wasCalled = false;
            var subscription = await _client.SubscribeAsync("#", _ =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await subscription.UnsubscribeAsync();
            await _client.PublishAsync(new MqttApplicationMessage {Topic = "idk"});
            await Task.Delay(1000);

            wasCalled.Should().BeFalse();
        }

        public async Task DisposeAsync()
        {
            await _client.DisposeAsync();
        }
    }
}