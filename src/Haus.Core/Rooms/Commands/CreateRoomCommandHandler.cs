using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;

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
        private readonly IMapper _mapper;

        public CreateRoomCommandHandler(HausDbContext context, IMapper mapper, IValidator<RoomModel> validator)
        {
            _context = context;
            _mapper = mapper;
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
            return _mapper.Map<RoomModel>(room);
        }
    }
}