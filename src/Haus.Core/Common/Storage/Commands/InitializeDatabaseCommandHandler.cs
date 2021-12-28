using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Common.Storage.Commands
{
    public record InitializeDatabaseCommand : ICommand;

    internal class InitializeDatabaseCommandHandler : AsyncRequestHandler<InitializeDatabaseCommand>, ICommandHandler<InitializeDatabaseCommand>
    {
        private readonly HausDbContext _context;

        public InitializeDatabaseCommandHandler(HausDbContext context)
        {
            _context = context;
        }

        protected override async Task Handle(InitializeDatabaseCommand request, CancellationToken cancellationToken)
        {
            var allMigrations = _context.Database.GetMigrations();
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken).ConfigureAwait(false);
            if (allMigrations.Count() != appliedMigrations.Count())
                await _context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}