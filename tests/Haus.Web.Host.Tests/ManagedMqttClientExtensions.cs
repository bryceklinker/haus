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
    }
}