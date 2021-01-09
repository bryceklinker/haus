using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Lighting;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands
{
    public record ChangeRoomLightingCommand(long RoomId, LightingModel Lighting) : ICommand;

    internal class ChangeRoomLightingCommandHandler : AsyncRequestHandler<ChangeRoomLightingCommand>, ICommandHandler<ChangeRoomLightingCommand>
    {
        private readonly IRoomCommandRepository _repository;
        private readonly IDomainEventBus _domainEventBus;

        public ChangeRoomLightingCommandHandler(IDomainEventBus domainEventBus, IRoomCommandRepository repository)
        {
            _domainEventBus = domainEventBus;
            _repository = repository;
        }

        protected override async Task Handle(ChangeRoomLightingCommand request, CancellationToken cancellationToken)
        {
            var room = await _repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);
            
            var lighting = LightingEntity.FromModel(request.Lighting);
            room.ChangeLighting(lighting, _domainEventBus);
            
            await _repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
            await _domainEventBus.FlushAsync(cancellationToken);
        }
    }
}