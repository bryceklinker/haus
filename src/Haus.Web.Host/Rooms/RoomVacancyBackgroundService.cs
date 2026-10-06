using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Commands;
using Haus.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Haus.Web.Host.Rooms;

public class RoomVacancyBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<RoomVacancyBackgroundService> logger
) : BackgroundService
{
    private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(500);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!await CanStartExecuting(stoppingToken).ConfigureAwait(false))
            await Task.Delay(200, stoppingToken).ConfigureAwait(false);

        while (!stoppingToken.IsCancellationRequested)
        {
            await TryExecuteTurnOffVacantRooms(stoppingToken).ConfigureAwait(false);
            await Task.Delay(_delay, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task TryExecuteTurnOffVacantRooms(CancellationToken stoppingToken)
    {
        try
        {
            await ExecuteTurnOffVacantRooms(stoppingToken).ConfigureAwait(false);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            // A single sweep racing a concurrent write to the same room (e.g. an explicit
            // vacant/occupied event landing at the same moment) must not permanently stop
            // this loop - HostOptions.BackgroundServiceExceptionBehavior defaults to
            // StopHost, so an unhandled exception here would silently disable vacancy
            // turn-off for every room for the rest of the process lifetime. Skip this
            // sweep and retry on the next poll instead.
            logger.LogWarning(e, "Skipping this vacancy sweep after a failure; will retry on the next poll");
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
