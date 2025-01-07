using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands;

public record TurnRoomOffCommand(long RoomId) : ICommand;

internal class TurnRoomOffCommandHandler(IDomainEventBus domainEventBus, IRoomCommandRepository repository)
    : ICommandHandler<TurnRoomOffCommand>
{
    public async Task Handle(TurnRoomOffCommand request, CancellationToken cancellationToken)
    {
        var room = await repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);
        room.TurnOff(domainEventBus);
        await repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}