using System;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Tests
{
    public static class ManagedMqttClientExtensions
    {
        public static async Task PublishAsync(this IManagedMqttClient client, string topic, object payload)
        {
            await client.PublishAsync(new MqttApplicationMessage
            {
                Topic = topic,
                Payload = JsonSerializer.SerializeToUtf8Bytes(payload)
            });
        }

        public static async Task SubscribeToAllTopicsAsync(this IManagedMqttClient client, Action<MqttApplicationMessage> handler)
        {
            await client.SubscribeToTopicAsync("#", handler);
        }
        
        public static async Task SubscribeToTopicAsync(this IManagedMqttClient client, string topic, Action<MqttApplicationMessage> handler)
        {
            await client.SubscribeAsync(topic);
            client.UseApplicationMessageReceivedHandler(args =>
            {
                if (args.ApplicationMessage.Topic == topic || topic == "#") 
                    handler(args.ApplicationMessage);
            });
        }
    }
}