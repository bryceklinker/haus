using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Rooms.Commands;
using Haus.Cqrs.Commands;
using Haus.Cqrs.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host.Rooms;

public class RoomVacancyBackgroundService : BackgroundService
{
    private readonly ICommandBus _commandBus;

    public RoomVacancyBackgroundService(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _commandBus.ExecuteAsync(new TurnOffVacantRoomsCommand(), stoppingToken);
            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
        }
    }
}