using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;

namespace Haus.Core.Rooms.Validators
{
    public class RoomModelValidator : AbstractValidator<RoomModel>
    {
        private readonly HausDbContext _context;

        public RoomModelValidator(HausDbContext context)
        {
            _context = context;
            RuleFor(r => r.Name)
                .Required()
                .MustAsync(BeUniqueAsync);
            RuleFor(r => r.OccupancyTimeoutInSeconds)
                .GreaterThanOrEqualTo(0);
        }

        private Task<bool> BeUniqueAsync(RoomModel model, string name, CancellationToken token)
        {
            return _context.IsUniqueAsync<RoomEntity, string>(
                model.Id,
                name,
                e => e.Name,
                token);
        }
    }
}