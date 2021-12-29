using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models.Discovery;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Discovery.Commands;

public record SyncDiscoveryCommand : ICommand;

internal class SyncDiscoveryCommandHandler : AsyncRequestHandler<SyncDiscoveryCommand>,
    ICommandHandler<SyncDiscoveryCommand>
{
    private readonly IHausBus _hausBus;

    public SyncDiscoveryCommandHandler(IHausBus hausBus)
    {
        _hausBus = hausBus;
    }

    protected override Task Handle(SyncDiscoveryCommand request, CancellationToken cancellationToken)
    {
        return _hausBus.PublishAsync(RoutableCommand.FromEvent(new SyncDiscoveryModel()), cancellationToken);
    }
}