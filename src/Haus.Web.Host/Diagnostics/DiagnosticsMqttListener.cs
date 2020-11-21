using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Diagnostics.Factories;
using Haus.Web.Host.Common.Mqtt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Diagnostics
{
    public class DiagnosticsMqttListener : BackgroundService
    {
        private readonly IMqttClientCreator _mqttClientCreator;
        private readonly IMqttDiagnosticsMessageFactory _messageFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DiagnosticsMqttListener> _logger;

        public DiagnosticsMqttListener(IMqttClientCreator mqttClientCreator,
            IServiceScopeFactory scopeFactory,
            IMqttDiagnosticsMessageFactory messageFactory,
            ILogger<DiagnosticsMqttListener> logger)
        {
            _mqttClientCreator = mqttClientCreator;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _messageFactory = messageFactory;
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
            using var scope = _scopeFactory.CreateScope();
            var hub = scope.ServiceProvider.GetRequiredService<IHubContext<DiagnosticsHub>>();
            var model = _messageFactory.Create(arg.ApplicationMessage.Topic, arg.ApplicationMessage.Payload);
            await hub.Clients.All.SendAsync("OnMqttMessage", model);
            _logger.LogInformation("Broadcast mqtt message to clients");
        }
    }
}