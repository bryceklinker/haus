using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Entities;
using MediatR;

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
        private readonly IHausBus _hausBus;
        private readonly IMapper _mapper;

        public ChangeRoomLightingCommandHandler(HausDbContext context, IMapper mapper, IHausBus hausBus)
        {
            _context = context;
            _mapper = mapper;
            _hausBus = hausBus;
        }

        protected override async Task Handle(ChangeRoomLightingCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.FindByIdAsync<RoomEntity>(request.RoomId, cancellationToken);
            if (room == null)
                throw new EntityNotFoundException<RoomEntity>(request.RoomId);
            
            var lighting = Lighting.FromModel(request.Lighting);
            room.ChangeLighting(lighting);
            await _context.SaveChangesAsync(cancellationToken);

            var @event = new RoomLightingChanged(_mapper.Map<RoomModel>(room), request.Lighting);
            await _hausBus.PublishAsync(new RoutableCommand(@event.AsHausCommand()), cancellationToken);
        }
    }
}