using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands;

public record TurnRoomOnCommand(long RoomId) : ICommand;

internal class TurnRoomOnCommandHandler : AsyncRequestHandler<TurnRoomOnCommand>, ICommandHandler<TurnRoomOnCommand>
{
    private readonly IRoomCommandRepository _repository;
    private readonly IDomainEventBus _domainEventBus;

    public TurnRoomOnCommandHandler(IDomainEventBus domainEventBus, IRoomCommandRepository repository)
    {
        _domainEventBus = domainEventBus;
        _repository = repository;
    }

    protected override async Task Handle(TurnRoomOnCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);
        room.TurnOn(_domainEventBus);
        await _repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}