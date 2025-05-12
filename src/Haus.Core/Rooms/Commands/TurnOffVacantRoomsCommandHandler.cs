using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Commands;

public record TurnOffVacantRoomsCommand : ICommand;

public class TurnOffVacantRoomsCommandHandler(IDomainEventBus domainEventBus, HausDbContext context)
    : ICommandHandler<TurnOffVacantRoomsCommand>
{
    public async Task Handle(TurnOffVacantRoomsCommand request, CancellationToken cancellationToken)
    {
        var rooms = await context
            .GetRoomsIncludeDevices()
            .Where(r => r.Lighting != null && r.Lighting.State == LightingState.On)
            .Where(r => r.LastOccupiedTime.HasValue)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);
        foreach (var room in rooms)
            room.ChangeOccupancy(
                new OccupancyChangedModel(RoomDefaults.SimulatedOccupancyChangeDeviceId),
                domainEventBus
            );

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
