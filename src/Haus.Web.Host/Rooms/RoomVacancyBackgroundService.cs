using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Commands;
using Haus.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host.Rooms;

public class RoomVacancyBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(500);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!await CanStartExecuting(stoppingToken).ConfigureAwait(false))
            await Task.Delay(200, stoppingToken).ConfigureAwait(false);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecuteTurnOffVacantRooms(stoppingToken).ConfigureAwait(false);
            await Task.Delay(_delay, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task<bool> CanStartExecuting(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        await using var db = scope.GetService<HausDbContext>();
        return await db.HaveMigrationsBeenApplied(stoppingToken).ConfigureAwait(false);
    }

    private async Task ExecuteTurnOffVacantRooms(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        await using var db = scope.GetService<HausDbContext>();
        var commandBus = scope.GetService<ICommandBus>();
        await commandBus.ExecuteAsync(new TurnOffVacantRoomsCommand(), stoppingToken).ConfigureAwait(false);
    }
}
