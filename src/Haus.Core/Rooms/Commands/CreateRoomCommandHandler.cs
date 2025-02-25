using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Entities;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs;
using Haus.Cqrs.Commands;

namespace Haus.Core.Rooms.Commands;

public record CreateRoomCommand(RoomModel Model) : CreateEntityCommand<RoomModel>(Model);

internal class CreateRoomCommandHandler(
    IValidator<RoomModel> validator,
    IRoomCommandRepository repository,
    IHausBus hausBus
) : ICommandHandler<CreateRoomCommand, RoomModel>
{
    public async Task<RoomModel> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        await validator.HausValidateAndThrowAsync(request.Model, cancellationToken).ConfigureAwait(false);
        var room = await repository
            .AddAsync(RoomEntity.CreateFromModel(request.Model), cancellationToken)
            .ConfigureAwait(false);

        var model = room.ToModel();
        await hausBus
            .PublishAsync(RoutableEvent.FromEvent(new RoomCreatedEvent(model)), cancellationToken)
            .ConfigureAwait(false);
        return model;
    }
}
