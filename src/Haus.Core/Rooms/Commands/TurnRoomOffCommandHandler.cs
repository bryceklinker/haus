using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands;

public record TurnRoomOffCommand(long RoomId) : ICommand;

internal class TurnRoomOffCommandHandler : ICommandHandler<TurnRoomOffCommand>
{
    private readonly IRoomCommandRepository _repository;
    private readonly IDomainEventBus _domainEventBus;

    public TurnRoomOffCommandHandler(IDomainEventBus domainEventBus, IRoomCommandRepository repository)
    {
        _domainEventBus = domainEventBus;
        _repository = repository;
    }

    public async Task Handle(TurnRoomOffCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);
        room.TurnOff(_domainEventBus);
        await _repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}