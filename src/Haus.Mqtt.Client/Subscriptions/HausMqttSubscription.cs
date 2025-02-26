using System;
using System.Threading.Tasks;
using MQTTnet;

namespace Haus.Mqtt.Client.Subscriptions;

public interface IHausMqttSubscription
{
    Guid Id { get; }
    Task ExecuteAsync(MqttApplicationMessage message);
    Task UnsubscribeAsync();
}

public class HausMqttSubscription(
    string topic,
    Func<MqttApplicationMessage, Task> handler,
    Func<IHausMqttSubscription, Task>? unsubscribe = null
) : IHausMqttSubscription
{
    public Guid Id { get; } = Guid.NewGuid();

    private bool IsSubscribedToTopic(string topic1)
    {
        return topic1 == topic || topic == "#";
    }

    public async Task ExecuteAsync(MqttApplicationMessage message)
    {
        if (IsSubscribedToTopic(message.Topic))
            await handler.Invoke(message);
    }

    public async Task UnsubscribeAsync()
    {
        if (unsubscribe == null)
            return;

        await unsubscribe.Invoke(this).ConfigureAwait(false);
    }
}
