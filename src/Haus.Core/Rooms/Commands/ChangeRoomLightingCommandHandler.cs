using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands;

public record ChangeRoomLightingCommand(long RoomId, LightingModel Lighting) : ICommand;

internal class ChangeRoomLightingCommandHandler(IDomainEventBus domainEventBus, IRoomCommandRepository repository)
    : ICommandHandler<ChangeRoomLightingCommand>
{
    public async Task Handle(ChangeRoomLightingCommand request, CancellationToken cancellationToken)
    {
        var room = await repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);

        var lighting = LightingEntity.FromModel(request.Lighting);
        room.ChangeLighting(lighting, domainEventBus);

        await repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
