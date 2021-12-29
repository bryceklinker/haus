using System;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Health;
using Haus.Mqtt.Client;

namespace Haus.Testing.Support;

public static class HausMqttClientExtensions
{
    public static async Task SubscribeToHausEventsAsync<T>(
        this IHausMqttClient client,
        string eventType,
        Action<HausEvent<T>> handler,
        string topicName = DefaultHausMqttTopics.EventsTopic)
    {
        await client.SubscribeAsync(topicName, msg =>
        {
            HausJsonSerializer.TryDeserialize(msg.Payload, out HausEvent<T> evt);
            if (evt?.Type == eventType) handler.Invoke(evt);
        });
    }

    public static async Task SubscribeToHausCommandsAsync<T>(
        this IHausMqttClient client,
        string commandType,
        Action<HausCommand<T>> handler,
        string topicName = DefaultHausMqttTopics.CommandsTopic)
    {
        await client.SubscribeAsync(topicName, msg =>
        {
            HausJsonSerializer.TryDeserialize(msg.Payload, out HausCommand<T> command);
            if (command?.Type == commandType) handler.Invoke(command);
        });
    }

    public static async Task SubscribeToHausHealthAsync(
        this IHausMqttClient client,
        Action<HausHealthReportModel> handler,
        string topicName = DefaultHausMqttTopics.HealthTopic)
    {
        await client.SubscribeAsync(topicName, msg =>
        {
            if (HausJsonSerializer.TryDeserialize(msg.Payload, out HausHealthReportModel report))
                handler.Invoke(report);
        });
    }
}