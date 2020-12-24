using System;
using System.Text.Json;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Mqtt.Client;
using Haus.Web.Host.Common.Mqtt;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Tests
{
    public static class HausMqttClientExtensions
    {
        public static async Task PublishAsync(this IHausMqttClient client, string topic, object payload)
        {
            await client.PublishAsync(new MqttApplicationMessage
            {
                Topic = topic,
                Payload = HausJsonSerializer.SerializeToBytes(payload)
            });
        }

        public static async Task SubscribeToAllTopicsAsync(this IHausMqttClient client, Action<MqttApplicationMessage> handler)
        {
            await client.SubscribeToTopicAsync("#", handler);
        }
        
        public static async Task SubscribeToTopicAsync(this IHausMqttClient client, string topic, Action<MqttApplicationMessage> handler)
        {
            await client.SubscribeAsync(topic, msg =>
            {
                handler.Invoke(msg);
                return Task.CompletedTask;
            });
        }
    }
}