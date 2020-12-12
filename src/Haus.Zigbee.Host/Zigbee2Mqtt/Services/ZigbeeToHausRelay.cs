using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Extensions;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Services
{
    public class ZigbeeToHausRelay : BackgroundService
    {
        private readonly IMqttFactory _mqttFactory;
        private readonly ILogger<ZigbeeToHausRelay> _logger;
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
        private readonly IMqttMessageMapper _mqttMessageMapper;

        private string ZigbeeMqttServerUrl => _zigbeeOptions.Value.Config.Mqtt.Server;
        private IManagedMqttClient MqttClient { get; set; }

        public ZigbeeToHausRelay(
            IOptions<ZigbeeOptions> zigbeeOptions,
            IMqttMessageMapper mqttMessageMapper,
            IMqttFactory mqttFactory,
            ILogger<ZigbeeToHausRelay> logger)
        {
            _zigbeeOptions = zigbeeOptions;
            _mqttMessageMapper = mqttMessageMapper;
            _mqttFactory = mqttFactory;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            MqttClient = _mqttFactory.CreateManagedMqttClient();
            await MqttClient.StartAsync(CreateMqttOptions(ZigbeeMqttServerUrl));
            await MqttClient.SubscribeAsync("#");
            MqttClient.UseApplicationMessageReceivedHandler(HandleMqttMessage);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await MqttClient.StopAsync();
            MqttClient.Dispose();
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task HandleMqttMessage(MqttApplicationMessageReceivedEventArgs arg)
        {
            var messageToSend = _mqttMessageMapper.Map(arg.ApplicationMessage);
            if (messageToSend == null)
                return;

            foreach (var message in messageToSend)
            {
                _logger.LogInformation($"Sending message to {message.Topic}...");
                await MqttClient.PublishAsync(message).ConfigureAwait(false);
                _logger.LogInformation($"Sent message to {message.Topic}.");    
            }
            
        }

        private static IManagedMqttClientOptions CreateMqttOptions(string mqttServerUrl)
        {
            return new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnect()
                .WithClientOptions(b => b.WithConnectionUri(new Uri(mqttServerUrl)))
                .Build();
        }
    }
}