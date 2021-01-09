using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Lighting.Entities;
using Haus.Cqrs.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Lighting.Commands
{
    public record InitializeDefaultLightingSettingsCommand : ICommand;

    internal class InitializeDefaultLightingSettingsCommandHandler : AsyncRequestHandler<InitializeDefaultLightingSettingsCommand>, ICommandHandler<InitializeDefaultLightingSettingsCommand>
    {
        private readonly HausDbContext _context;

        public InitializeDefaultLightingSettingsCommandHandler(HausDbContext context)
        {
            _context = context;
        }

        protected override async Task Handle(InitializeDefaultLightingSettingsCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Set<DefaultLightingConstraintsEntity>().AnyAsync(cancellationToken))
                return;
            
            _context.Add(new DefaultLightingConstraintsEntity());
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}