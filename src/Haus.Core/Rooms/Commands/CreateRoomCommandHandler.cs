using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;

namespace Haus.Core.Rooms.Commands
{
    public record CreateRoomCommand(RoomModel Model) : CreateEntityCommand<RoomModel>(Model);

    internal class CreateRoomCommandHandler : ICommandHandler<CreateRoomCommand, RoomModel>
    {
        private readonly ICommandRoomRepository _repository;
        private readonly IValidator<RoomModel> _validator;

        public CreateRoomCommandHandler(IValidator<RoomModel> validator, ICommandRoomRepository repository)
        {
            _validator = validator;
            _repository = repository;
        }

        public async Task<RoomModel> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            await _validator.HausValidateAndThrowAsync(request.Model, cancellationToken)
                .ConfigureAwait(false);
            var room = await _repository.AddAsync(RoomEntity.CreateFromModel(request.Model), cancellationToken)
                .ConfigureAwait(false);
            return room.ToModel();
        }
    }
}