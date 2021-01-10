using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host.DeviceSimulator
{
    public class DeviceSimulatorStatePublisher : BackgroundService
    {
        private readonly IDeviceSimulatorStore _simulatorStore;
        private readonly IServiceScopeFactory _scopeFactory;
        private IDisposable _subscription;

        public DeviceSimulatorStatePublisher(
            IDeviceSimulatorStore simulatorStore, 
            IServiceScopeFactory scopeFactory)
        {
            _simulatorStore = simulatorStore;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscription = _simulatorStore.SubscribeAsync(PublishSimulatorStateAsync);
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            return base.StopAsync(cancellationToken);
        }

        private async Task PublishSimulatorStateAsync(IDeviceSimulatorState state)
        {
            using var scope = _scopeFactory.CreateScope();
            var hubContext = scope.GetService<IHubContext<DeviceSimulatorHub>>();
            await hubContext.BroadcastAsync("OnState", state.ToModel()).ConfigureAwait(false);
        }
    }
}