using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage.Commands;
using Haus.Core.Discovery.Commands;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Common.Commands;

public record InitializeCommand : ICommand;

internal class InitializeCommandHandler : AsyncRequestHandler<InitializeCommand>, ICommandHandler<InitializeCommand>
{
    private readonly IHausBus _hausBus;

    public InitializeCommandHandler(IHausBus hausBus)
    {
        _hausBus = hausBus;
    }

    protected override async Task Handle(InitializeCommand request, CancellationToken cancellationToken)
    {
        await _hausBus.ExecuteCommandAsync(new InitializeDatabaseCommand(), cancellationToken)
            .ConfigureAwait(false);
        await Task.WhenAll(
            _hausBus.ExecuteCommandAsync(new InitializeDiscoveryCommand(), cancellationToken),
            _hausBus.ExecuteCommandAsync(new SyncDiscoveryCommand(), cancellationToken)
        ).ConfigureAwait(false);
    }
}