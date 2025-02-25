using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Common.Storage.Commands;

public record InitializeDatabaseCommand : ICommand;

internal class InitializeDatabaseCommandHandler(HausDbContext context) : ICommandHandler<InitializeDatabaseCommand>
{
    public async Task Handle(InitializeDatabaseCommand request, CancellationToken cancellationToken)
    {
        var allMigrations = context.Database.GetMigrations();
        var appliedMigrations = await context
            .Database.GetAppliedMigrationsAsync(cancellationToken)
            .ConfigureAwait(false);
        if (allMigrations.Count() != appliedMigrations.Count())
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }
}
