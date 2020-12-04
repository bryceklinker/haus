using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using MediatR;

namespace Haus.Core.Rooms.Commands
{
    public class UpdateRoomCommand : UpdateEntityCommand<RoomModel>
    {
        public UpdateRoomCommand(long id, RoomModel model) 
            : base(id, model)
        {
        }
    }

    internal class UpdateRoomCommandHandler : AsyncRequestHandler<UpdateRoomCommand>, ICommandHandler<UpdateRoomCommand>
    {
        private readonly HausDbContext _context;
        private readonly IValidator<RoomModel> _validator;

        public UpdateRoomCommandHandler(HausDbContext context, IValidator<RoomModel> validator)
        {
            _context = context;
            _validator = validator;
        }

        protected override async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            await _validator.HausValidateAndThrowAsync(request.Model, cancellationToken);
               
            var room = await _context.FindByIdAsync<RoomEntity>(request.Id, cancellationToken);
            if (room == null)
                throw new EntityNotFoundException<RoomEntity>(request.Id);
            
            room.UpdateFromModel(request.Model);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}