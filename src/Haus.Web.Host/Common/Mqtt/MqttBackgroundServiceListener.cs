using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Haus.Web.Host.Common.Mqtt
{
    public abstract class MqttBackgroundServiceListener : BackgroundService
    {
        private readonly IHausMqttClientFactory _hausMqttClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;

        public MqttBackgroundServiceListener(
            IHausMqttClientFactory hausMqttClientFactory,
            IServiceScopeFactory scopeFactory,
            ILogger logger)
        {
            _hausMqttClientFactory = hausMqttClientFactory;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var mqttClient = await _hausMqttClientFactory.CreateClient();
            await mqttClient.SubscribeAsync("#", OnMessageReceived);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        protected IServiceScope CreateScope()
        {
            return _scopeFactory.CreateScope();
        }
        
        protected abstract Task OnMessageReceived(MqttApplicationMessage message);
    }
}