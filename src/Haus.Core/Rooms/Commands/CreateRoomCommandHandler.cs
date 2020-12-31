using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Commands;

namespace Haus.Core.Rooms.Commands
{
    public class CreateRoomCommand : CreateEntityCommand<RoomModel>
    {
        public CreateRoomCommand(RoomModel model)
            : base(model)
        {
        }
    }

    internal class CreateRoomCommandHandler : ICommandHandler<CreateRoomCommand, RoomModel>
    {
        private readonly HausDbContext _context;
        private readonly IValidator<RoomModel> _validator;

        public CreateRoomCommandHandler(HausDbContext context, IValidator<RoomModel> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<RoomModel> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            await _validator.HausValidateAndThrowAsync(request.Model, cancellationToken)
                .ConfigureAwait(false);
            var room = RoomEntity.CreateFromModel(request.Model);
            _context.Add(room);
            await _context.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
            return room.ToModel();
        }
    }
}