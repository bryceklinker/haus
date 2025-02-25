using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage.Commands;
using Haus.Core.Discovery.Commands;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Common.Commands;

public record InitializeCommand : ICommand;

internal class InitializeCommandHandler(IHausBus hausBus) : ICommandHandler<InitializeCommand>
{
    public async Task Handle(InitializeCommand request, CancellationToken cancellationToken)
    {
        await hausBus.ExecuteCommandAsync(new InitializeDatabaseCommand(), cancellationToken).ConfigureAwait(false);
        await Task.WhenAll(
                hausBus.ExecuteCommandAsync(new InitializeDiscoveryCommand(), cancellationToken),
                hausBus.ExecuteCommandAsync(new SyncDiscoveryCommand(), cancellationToken)
            )
            .ConfigureAwait(false);
    }
}
