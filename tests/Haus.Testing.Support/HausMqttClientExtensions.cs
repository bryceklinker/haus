using System;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Health;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Health;
using MQTTnet;

namespace Haus.Testing.Support
{
    public static class HausMqttClientExtensions
    {
        public static async Task SubscribeToHausEventsAsync<T>(
            this IHausMqttClient client, 
            Action<HausEvent<T>> handler, 
            string topicName = DefaultHausMqttTopics.EventsTopic)
        {
            await client.SubscribeAsync(topicName, msg =>
            {
                if (HausJsonSerializer.TryDeserialize(msg.Payload, out HausEvent<T> command))
                {
                    handler.Invoke(command);
                }
            });
        }

        public static async Task SubscribeToHausCommandsAsync<T>(
            this IHausMqttClient client, 
            Action<HausCommand<T>> handler,
            string topicName = DefaultHausMqttTopics.CommandsTopic)
        {
            await client.SubscribeAsync(topicName, msg =>
            {
                if (HausJsonSerializer.TryDeserialize(msg.Payload, out HausCommand<T> command))
                {
                    handler.Invoke(command);
                }
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
                {
                    handler.Invoke(report);
                }
            });
        }
    }
}