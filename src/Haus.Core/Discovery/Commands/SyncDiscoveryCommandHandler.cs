using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models.Discovery;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Discovery.Commands;

public record SyncDiscoveryCommand : ICommand;

internal class SyncDiscoveryCommandHandler(IHausBus hausBus) : ICommandHandler<SyncDiscoveryCommand>
{
    public Task Handle(SyncDiscoveryCommand request, CancellationToken cancellationToken)
    {
        return hausBus.PublishAsync(RoutableCommand.FromEvent(new SyncDiscoveryModel()), cancellationToken);
    }
}
