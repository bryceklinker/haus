using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Commands
{
    public class TurnRoomOnCommand : ICommand
    {
        public long RoomId { get; }

        public TurnRoomOnCommand(long roomId)
        {
            RoomId = roomId;
        }
    }

    internal class TurnRoomOnCommandHandler : AsyncRequestHandler<TurnRoomOnCommand>, ICommandHandler<TurnRoomOnCommand>
    {
        private readonly HausDbContext _context;
        private readonly IDomainEventBus _domainEventBus;

        public TurnRoomOnCommandHandler(HausDbContext context, IDomainEventBus domainEventBus)
        {
            _context = context;
            _domainEventBus = domainEventBus;
        }

        protected override async Task Handle(TurnRoomOnCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.FindByIdOrThrowAsync<RoomEntity>(request.RoomId, 
                query => query.Include(r => r.Devices), 
                cancellationToken).ConfigureAwait(false);
            room.TurnOn(_domainEventBus);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}