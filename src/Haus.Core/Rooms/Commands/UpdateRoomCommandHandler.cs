using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Rooms.Commands;

public record UpdateRoomCommand(RoomModel Model) : UpdateEntityCommand<RoomModel>(Model);

internal class UpdateRoomCommandHandler(
    IValidator<RoomModel> validator,
    IRoomCommandRepository repository,
    IHausBus hausBus)
    : ICommandHandler<UpdateRoomCommand>
{
    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        await validator.HausValidateAndThrowAsync(request.Model, cancellationToken)
            .ConfigureAwait(false);

        var room = await repository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

        room.UpdateFromModel(request.Model);
        await repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await hausBus.PublishAsync(RoutableEvent.FromEvent(new RoomUpdatedEvent(room.ToModel())), cancellationToken)
            .ConfigureAwait(false);
    }
}