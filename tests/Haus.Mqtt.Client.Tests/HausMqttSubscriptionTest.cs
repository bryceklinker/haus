using System.Threading.Tasks;
using FluentAssertions;
using Haus.Mqtt.Client.Tests.Support;
using Haus.Testing.Support;
using MQTTnet;
using Xunit;

namespace Haus.Mqtt.Client.Tests
{
    public class HausMqttSubscriptionTest : IAsyncLifetime
    {
        private IHausMqttClient _client;

        public async Task InitializeAsync()
        {
            _client = await new SupportFactory().CreateClient();
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