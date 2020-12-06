using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Common.Mqtt
{
    public interface IHausMqttClientFactory : IAsyncDisposable
    {
        Task<IHausMqttClient> CreateClient();
    }

    public class HausMqttClientFactory : IHausMqttClientFactory
    {
        private readonly IOptions<MqttOptions> _options;
        private readonly IMqttFactory _mqttFactory;
        private readonly IMqttNetLogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Lazy<Task<IHausMqttClient>> _clientInitializer;

        private string MqttServer => _options.Value.Server;
        
        public HausMqttClientFactory(IOptions<MqttOptions> options, IMqttFactory mqttFactory, IMqttNetLogger logger, ILoggerFactory loggerFactory)
        {
            _options = options;
            _mqttFactory = mqttFactory;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _clientInitializer = new Lazy<Task<IHausMqttClient>>(CreateMqttClient);
        }

        public async Task<IHausMqttClient> CreateClient()
        {
            return await _clientInitializer.Value;
        }

        private async Task<IHausMqttClient> CreateMqttClient()
        {
            var client = _mqttFactory.CreateManagedMqttClient(_logger);
            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(opts =>
                {
                    opts.WithConnectionUri(new Uri(MqttServer));
                })
                .Build();
            await client.StartAsync(options);
            return new HausMqttClient(client, _loggerFactory);
        }
        
        public async ValueTask DisposeAsync()
        {
            if (!_clientInitializer.IsValueCreated)
                return;

            var client = await _clientInitializer.Value;
            await client.DisposeAsync();
        }
    }
}