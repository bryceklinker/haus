using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Rooms.Commands
{
    public record UpdateRoomCommand(RoomModel Model) : UpdateEntityCommand<RoomModel>(Model);

    internal class UpdateRoomCommandHandler : AsyncRequestHandler<UpdateRoomCommand>, ICommandHandler<UpdateRoomCommand>
    {
        private readonly ICommandRoomRepository _repository;
        private readonly IValidator<RoomModel> _validator;

        public UpdateRoomCommandHandler(IValidator<RoomModel> validator, ICommandRoomRepository repository)
        {
            _validator = validator;
            _repository = repository;
        }

        protected override async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            await _validator.HausValidateAndThrowAsync(request.Model, cancellationToken)
                .ConfigureAwait(false);

            var room = await _repository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
            
            room.UpdateFromModel(request.Model);
            await _repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        }
    }
}