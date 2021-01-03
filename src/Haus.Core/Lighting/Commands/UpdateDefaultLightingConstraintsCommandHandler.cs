using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Lighting;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Lighting.Commands
{
    public record UpdateDefaultLightingConstraintsCommand(LightingConstraintsModel Model) : ICommand
    {
    }

    internal class UpdateDefaultLightingConstraintsCommandHandler : AsyncRequestHandler<UpdateDefaultLightingConstraintsCommand>, ICommandHandler<UpdateDefaultLightingConstraintsCommand>
    {
        private readonly HausDbContext _context;

        public UpdateDefaultLightingConstraintsCommandHandler(HausDbContext context)
        {
            _context = context;
        }

        protected override async Task Handle(UpdateDefaultLightingConstraintsCommand request, CancellationToken cancellationToken)
        {
            var constraints = await _context.GetDefaultLightingConstraintsAsync(cancellationToken).ConfigureAwait(false);
            constraints.UpdateFromModel(request.Model);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}