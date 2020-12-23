using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Haus.Mqtt.Client.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Mqtt.Client
{
    public interface IHausMqttClientFactory : IAsyncDisposable
    {
        Task<IHausMqttClient> CreateClient();
        Task<IHausMqttClient> CreateClient(string url);
    }

    public class HausMqttClientFactory : IHausMqttClientFactory
    {
        private readonly IOptions<HausMqttSettings> _options;
        private readonly IMqttFactory _mqttFactory;
        private readonly IMqttNetLogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, Task<IHausMqttClient>> _clients;

        private string MqttServer => _options.Value.Server;
        
        public HausMqttClientFactory(IOptions<HausMqttSettings> options, IMqttFactory mqttFactory, IMqttNetLogger logger, ILoggerFactory loggerFactory)
        {
            _options = options;
            _mqttFactory = mqttFactory;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _clients = new ConcurrentDictionary<string, Task<IHausMqttClient>>();
        }

        public Task<IHausMqttClient> CreateClient()
        {
            return CreateClient(MqttServer);
        }

        public async Task<IHausMqttClient> CreateClient(string url)
        {
            return await _clients.GetOrAdd(url, CreateMqttClient).ConfigureAwait(false);
        }

        private async Task<IHausMqttClient> CreateMqttClient(string url)
        {
            var client = _mqttFactory.CreateManagedMqttClient(_logger);
            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(opts =>
                {
                    opts.WithConnectionUri(new Uri(url));
                })
                .Build();
            await client.StartAsync(options).ConfigureAwait(false);
            return new HausMqttClient(client, _loggerFactory);
        }
        
        public async ValueTask DisposeAsync()
        {
            foreach (var client in _clients) 
                await client.Value.Result.DisposeAsync().ConfigureAwait(false);
        }
    }
}