using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands
{
    public class TurnRoomOffCommand : ICommand
    {
        public long RoomId { get; }

        public TurnRoomOffCommand(long roomId)
        {
            RoomId = roomId;
        }
    }

    internal class TurnRoomOffCommandHandler : AsyncRequestHandler<TurnRoomOffCommand>, ICommandHandler<TurnRoomOffCommand>
    {
        private readonly HausDbContext _context;
        private readonly IDomainEventBus _domainEventBus;

        public TurnRoomOffCommandHandler(HausDbContext context, IDomainEventBus domainEventBus)
        {
            _context = context;
            _domainEventBus = domainEventBus;
        }

        protected override async Task Handle(TurnRoomOffCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.FindByIdOrThrowAsync<RoomEntity>(request.RoomId, cancellationToken).ConfigureAwait(false);
            room.TurnOff(_domainEventBus);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}