using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands
{
    public record TurnRoomOffCommand(long RoomId) : ICommand;

    internal class TurnRoomOffCommandHandler : AsyncRequestHandler<TurnRoomOffCommand>, ICommandHandler<TurnRoomOffCommand>
    {
        private readonly ICommandRoomRepository _repository;
        private readonly IDomainEventBus _domainEventBus;

        public TurnRoomOffCommandHandler(IDomainEventBus domainEventBus, ICommandRoomRepository repository)
        {
            _domainEventBus = domainEventBus;
            _repository = repository;
        }

        protected override async Task Handle(TurnRoomOffCommand request, CancellationToken cancellationToken)
        {
            var room = await _repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);
            room.TurnOff(_domainEventBus);
            await _repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
            await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}