using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Tests.Support
{
    public static class MqttSupport
    {
        public static async Task PublishMessageAsync(string topic, object payload)
        {
            var message = new MqttApplicationMessage
            {
                Topic = topic,
                Payload = JsonSerializer.SerializeToUtf8Bytes(payload)
            };
            using var client = await CreateClient();
            await client.PublishAsync(message);
        }

        private static async Task<IManagedMqttClient> CreateClient()
        {
            var factory = new MqttFactory();
            var client = factory.CreateManagedMqttClient();
            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(opts =>
                {
                    opts.WithTcpServer("localhost");
                })
                .Build();
            await client.StartAsync(options);
            return client;
        }
    }
}