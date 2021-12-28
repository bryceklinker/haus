using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Rooms.Commands;
using Haus.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host.Rooms;

public class RoomVacancyBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RoomVacancyBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var commandBus = scope.GetService<ICommandBus>();
            await commandBus.ExecuteAsync(new TurnOffVacantRoomsCommand(), stoppingToken);
            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
        }
    }
}