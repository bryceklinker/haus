using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Common.Mqtt
{
    public interface IMqttClientCreator : IAsyncDisposable
    {
        Task<IManagedMqttClient> CreateClient();
    }

    public class MqttClientCreator : IMqttClientCreator
    {
        private readonly IOptions<MqttOptions> _options;
        private readonly IMqttNetLogger _logger;
        private readonly Lazy<Task<IManagedMqttClient>> _clientInitializer;

        private string MqttServer => _options.Value.Server;
        
        public MqttClientCreator(IOptions<MqttOptions> options, IMqttNetLogger logger)
        {
            _options = options;
            _logger = logger;
            _clientInitializer = new Lazy<Task<IManagedMqttClient>>(CreateMqttClient);
        }

        public async Task<IManagedMqttClient> CreateClient()
        {
            return await _clientInitializer.Value;
        }

        private async Task<IManagedMqttClient> CreateMqttClient()
        {
            var factory = new MqttFactory(_logger);
            var client = factory.CreateManagedMqttClient(_logger);
            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(opts =>
                {
                    opts.WithConnectionUri(new Uri(MqttServer));
                })
                .Build();
            await client.StartAsync(options);
            return client;
        }
        
        public async ValueTask DisposeAsync()
        {
            if (!_clientInitializer.IsValueCreated)
                return;

            var client = await _clientInitializer.Value;
            // await client.StopAsync();
            client.Dispose();
        }
    }
}