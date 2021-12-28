using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Rooms;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Commands;

public record TurnOffVacantRoomsCommand() : ICommand;

public class TurnOffVacantRoomsCommandHandler : AsyncRequestHandler<TurnOffVacantRoomsCommand>, ICommandHandler<TurnOffVacantRoomsCommand>
{
    private readonly IDomainEventBus _domainEventBus;
    private readonly HausDbContext _context;

    public TurnOffVacantRoomsCommandHandler(IDomainEventBus domainEventBus, HausDbContext context)
    {
        _domainEventBus = domainEventBus;
        _context = context;
    }
    
    protected override async Task Handle(TurnOffVacantRoomsCommand request, CancellationToken cancellationToken)
    {
        var rooms = await _context.GetRoomsIncludeDevices().ToArrayAsync(cancellationToken).ConfigureAwait(false);
        foreach (var room in rooms)
        {
            room.ChangeOccupancy(
                new OccupancyChangedModel(RoomDefaults.SimulatedOccupancyChangeDeviceId),
                _domainEventBus);
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}