using System.Threading.Tasks;
using Haus.Mqtt.Client.Subscriptions;
using MQTTnet;
using Xunit;

namespace Haus.Mqtt.Client.Tests
{
    public class HausMqttSubscriptionTest
    {
        [Fact]
        public async Task WhenExecutedForMessageWithDifferentTopicThenSubscriberIsNotExecuted()
        {
            MqttApplicationMessage actual = null;
            var subscription = new HausMqttSubscription("one", msg =>
            {
                actual = msg;
                return Task.CompletedTask;
            });

            await subscription.ExecuteAsync(new MqttApplicationMessage
            {
                Topic = "other"
            });

            Assert.Null(actual);
        }

        [Fact]
        public async Task WhenSubscribedToAllTopicsThenExecuteAlwaysInvokesSubscriber()
        {
            MqttApplicationMessage actual = null;
            var subscription = new HausMqttSubscription("#", msg =>
            {
                actual = msg;
                return Task.CompletedTask;
            });
            
            var expected =new MqttApplicationMessage{Topic = "other"};
            await subscription.ExecuteAsync(expected);

            Assert.Same(expected, actual);
        }
    }
}