using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Haus.Device.Simulator.Devices.Services
{
    public class RealtimeDevicesService : BackgroundService
    {
        private readonly IDevicesStore _store;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RealtimeDevicesService> _logger;
        private IDisposable _subscription;

        public RealtimeDevicesService(
            IDevicesStore store,
            IServiceScopeFactory scopeFactory, 
            ILogger<RealtimeDevicesService> logger)
        {
            _store = store;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscription = _store.SubscribeAsync(PublishNewState);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription.Dispose();
            return base.StopAsync(cancellationToken);
        }

        private async Task PublishNewState(IDevicesState state)
        {
            _logger.LogInformation("Broadcasting state to clients...");
            using var scope = _scopeFactory.CreateScope();
            var hubService = scope.ServiceProvider.GetRequiredService<IDevicesHubService>();
            await hubService.PublishStateAsync(state).ConfigureAwait(false);
            _logger.LogInformation("Broadcast state to clients");
        }
    }
}