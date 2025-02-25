using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands;

public record TurnRoomOnCommand(long RoomId) : ICommand;

internal class TurnRoomOnCommandHandler(IDomainEventBus domainEventBus, IRoomCommandRepository repository)
    : ICommandHandler<TurnRoomOnCommand>
{
    public async Task Handle(TurnRoomOnCommand request, CancellationToken cancellationToken)
    {
        var room = await repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);
        room.TurnOn(domainEventBus);
        await repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
