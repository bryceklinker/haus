using System;
using System.Threading.Tasks;
using MQTTnet;

namespace Haus.Web.Host.Common.Mqtt
{
    public interface IHausMqttSubscription
    {
        Guid Id { get; }
        Task ExecuteAsync(MqttApplicationMessage message);
        Task UnsubscribeAsync();
    }
    
    public class HausMqttSubscription : IHausMqttSubscription
    {
        private readonly string _topic;
        private readonly Func<MqttApplicationMessage, Task> _handler;
        private readonly Func<IHausMqttSubscription, Task> _unsubscribe;

        public Guid Id { get; }

        public HausMqttSubscription(string topic, Func<MqttApplicationMessage,Task> handler, Func<IHausMqttSubscription, Task> unsubscribe = null)
        {
            Id = Guid.NewGuid();
            _topic = topic;
            _handler = handler;
            _unsubscribe = unsubscribe;
        }

        private bool IsSubscribedToTopic(string topic)
        {
            return topic == _topic || _topic == "#";
        }

        public async Task ExecuteAsync(MqttApplicationMessage message)
        {
            if (IsSubscribedToTopic(message.Topic)) await _handler.Invoke(message);
        }

        public async Task UnsubscribeAsync()
        {
            await _unsubscribe.Invoke(this);
        }
    }
}