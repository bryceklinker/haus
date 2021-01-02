using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Commands
{
    public class ChangeRoomLightingCommand : ICommand
    {
        public long RoomId { get; }
        public LightingModel Lighting { get; }

        public ChangeRoomLightingCommand(long roomId, LightingModel lighting)
        {
            RoomId = roomId;
            Lighting = lighting;
        }
    }

    internal class ChangeRoomLightingCommandHandler : AsyncRequestHandler<ChangeRoomLightingCommand>, ICommandHandler<ChangeRoomLightingCommand>
    {
        private readonly HausDbContext _context;
        private readonly IDomainEventBus _domainEventBus;

        public ChangeRoomLightingCommandHandler(HausDbContext context, IDomainEventBus domainEventBus)
        {
            _context = context;
            _domainEventBus = domainEventBus;
        }

        protected override async Task Handle(ChangeRoomLightingCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.FindByIdOrThrowAsync<RoomEntity>(request.RoomId, 
                query => query.Include(r => r.Devices), 
                cancellationToken);
            var lighting = Lighting.FromModel(request.Lighting);
            room.ChangeLighting(lighting, _domainEventBus);
            await _context.SaveChangesAsync(cancellationToken);
            
            await _domainEventBus.FlushAsync(cancellationToken);
        }
    }
}