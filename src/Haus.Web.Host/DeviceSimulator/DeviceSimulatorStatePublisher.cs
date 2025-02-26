using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core;
using Haus.Core.DeviceSimulator.State;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host.DeviceSimulator;

public class DeviceSimulatorStatePublisher(IDeviceSimulatorStore simulatorStore, IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private IDisposable? _subscription;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _subscription = simulatorStore.SubscribeAsync(PublishSimulatorStateAsync);
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Dispose();
        return base.StopAsync(cancellationToken);
    }

    private async Task PublishSimulatorStateAsync(IDeviceSimulatorState state)
    {
        using var scope = scopeFactory.CreateScope();
        var hubContext = scope.GetService<IHubContext<DeviceSimulatorHub>>();
        await hubContext.BroadcastAsync("OnState", state.ToModel()).ConfigureAwait(false);
    }
}
