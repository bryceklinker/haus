using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Haus.Mqtt.Client.Settings;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions;
using MQTTnet.Extensions.ManagedClient;
using Polly;

namespace Haus.Mqtt.Client
{
    public interface IHausMqttClientFactory : IAsyncDisposable
    {
        Task<IHausMqttClient> CreateClient();
        Task<IHausMqttClient> CreateClient(string url);
    }

    internal class HausMqttClientFactory : IHausMqttClientFactory
    {
        private readonly IOptions<HausMqttSettings> _options;
        private readonly IMqttFactory _mqttFactory;
        private readonly IMqttNetLogger _logger;
        private readonly ConcurrentDictionary<string, Task<IHausMqttClient>> _clients;

        private string MqttServer => _options.Value.Server;
        
        public HausMqttClientFactory(IOptions<HausMqttSettings> options, IMqttFactory mqttFactory, IMqttNetLogger logger)
        {
            _options = options;
            _mqttFactory = mqttFactory;
            _logger = logger;
            _clients = new ConcurrentDictionary<string, Task<IHausMqttClient>>();
        }

        public Task<IHausMqttClient> CreateClient()
        {
            return CreateClient(MqttServer);
        }

        public async Task<IHausMqttClient> CreateClient(string url)
        {
            return await _clients.GetOrAdd(url, CreateMqttClientWithRetry).ConfigureAwait(false);
        }

        private async Task<IHausMqttClient> CreateMqttClientWithRetry(string url)
        {
            var client = await CreateMqttClient(url);
            var retryCount = 1;
            while (!client.IsConnected && retryCount < 5)
            {
                await Task.Delay(10 * retryCount);
                retryCount++;
            }

            return client;
        }

        private async Task<IHausMqttClient> CreateMqttClient(string url)
        {
            var client = _mqttFactory.CreateManagedMqttClient(_logger);
            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(opts => { opts.WithConnectionUri(new Uri(url)); })
                .Build();
            await client.StartAsync(options).ConfigureAwait(false);
            return new HausMqttClient(client, _options);
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var client in _clients) 
                await client.Value.Result.DisposeAsync().ConfigureAwait(false);
        }
    }
}