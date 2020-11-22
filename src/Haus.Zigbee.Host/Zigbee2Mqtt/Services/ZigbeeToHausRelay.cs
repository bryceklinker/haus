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
        private readonly IZigbeeToHausModelMapper _zigbeeToHausModelMapper;

        private string ZigbeeMqttServerUrl => _zigbeeOptions.Value.Config.Mqtt.Server;
        private string ZigbeeBaseTopic => _zigbeeOptions.Value.Config.Mqtt.BaseTopic;
        private IManagedMqttClient ZigbeeMqttClient { get; set; }

        public ZigbeeToHausRelay(
            IOptions<ZigbeeOptions> zigbeeOptions,
            IZigbeeToHausModelMapper zigbeeToHausModelMapper,
            IMqttFactory mqttFactory,
            ILogger<ZigbeeToHausRelay> logger)
        {
            _zigbeeOptions = zigbeeOptions;
            _zigbeeToHausModelMapper = zigbeeToHausModelMapper;
            _mqttFactory = mqttFactory;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            ZigbeeMqttClient = _mqttFactory.CreateManagedMqttClient();
            await ZigbeeMqttClient.StartAsync(CreateMqttOptions(ZigbeeMqttServerUrl));
            await ZigbeeMqttClient.SubscribeAsync("#");
            ZigbeeMqttClient.UseApplicationMessageReceivedHandler(HandleMqttMessage);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await ZigbeeMqttClient.StopAsync();
            ZigbeeMqttClient.Dispose();
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
            if (!arg.ApplicationMessage.Topic.StartsWith(ZigbeeBaseTopic))
                return;

            var hausMessage = _zigbeeToHausModelMapper.Map(arg.ApplicationMessage);
            _logger.LogInformation($"Sending message to {hausMessage.Topic}...");
            await ZigbeeMqttClient.PublishAsync(hausMessage);
            _logger.LogInformation($"Sent message to {hausMessage.Topic}.");
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