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

internal class UpdateRoomCommandHandler : ICommandHandler<UpdateRoomCommand>
{
    private readonly IRoomCommandRepository _repository;
    private readonly IValidator<RoomModel> _validator;
    private readonly IHausBus _hausBus;

    public UpdateRoomCommandHandler(IValidator<RoomModel> validator, IRoomCommandRepository repository,
        IHausBus hausBus)
    {
        _validator = validator;
        _repository = repository;
        _hausBus = hausBus;
    }

    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        await _validator.HausValidateAndThrowAsync(request.Model, cancellationToken)
            .ConfigureAwait(false);

        var room = await _repository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

        room.UpdateFromModel(request.Model);
        await _repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(new RoomUpdatedEvent(room.ToModel())), cancellationToken)
            .ConfigureAwait(false);
    }
}