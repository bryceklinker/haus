using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Diagnostics.Factories;
using Haus.Web.Host.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Common.Mqtt
{
    public class MqttMessageListener : BackgroundService
    {
        private readonly IMqttClientCreator _mqttClientCreator;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MqttMessageListener> _logger;

        public MqttMessageListener(IMqttClientCreator mqttClientCreator,
            IServiceScopeFactory scopeFactory,
            ILogger<MqttMessageListener> logger)
        {
            _mqttClientCreator = mqttClientCreator;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var mqttClient = await _mqttClientCreator.CreateClient();
            await mqttClient.SubscribeAsync("#");
            mqttClient.UseConnectedHandler(OnConnected);
            mqttClient.UseApplicationMessageReceivedHandler(OnMessageReceived);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void OnConnected(MqttClientConnectedEventArgs arg)
        {
            _logger.LogInformation("Connected to mqtt");
        }

        private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            _logger.LogInformation("Received mqtt message");
            await RouteMqttMessage(arg.ApplicationMessage);
            await PublishToDiagnosticsAsync(arg.ApplicationMessage);
            _logger.LogInformation("Broadcast mqtt message to clients");
        }

        private async Task RouteMqttMessage(MqttApplicationMessage message)
        {
            using var scope = _scopeFactory.CreateScope();
            var eventFactory = scope.GetService<IRoutableEventFactory>();
            var @event = eventFactory.Create(message.Payload);
            if (@event == null)
                return;

            var eventBus = scope.GetService<IEventBus>();
            await eventBus.PublishAsync(@event, CancellationToken.None);
        }

        private async Task PublishToDiagnosticsAsync(MqttApplicationMessage message)
        {
            using var scope = _scopeFactory.CreateScope();
            var hub = scope.GetService<IHubContext<DiagnosticsHub>>();
            var messageFactory = scope.GetService<IMqttDiagnosticsMessageFactory>();
            var model = messageFactory.Create(message.Topic, message.Payload);
            await hub.Clients.All.SendAsync("OnMqttMessage", model);
        }
    }
}