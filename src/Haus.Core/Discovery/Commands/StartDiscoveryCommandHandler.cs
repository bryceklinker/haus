using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Discovery;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Discovery.Commands;

public record StartDiscoveryCommand : ICommand;

internal class StartDiscoveryCommandHandler(IHausBus hausBus, HausDbContext context)
    : ICommandHandler<StartDiscoveryCommand>
{
    public async Task Handle(StartDiscoveryCommand request, CancellationToken cancellationToken)
    {
        var model = new StartDiscoveryModel();

        var discovery = await context.GetDiscoveryEntityAsync(cancellationToken).ConfigureAwait(false);
        discovery?.Start();
        await context.SaveChangesAsync(cancellationToken);

        await Task.WhenAll(
                hausBus.PublishAsync(RoutableCommand.FromEvent(model), cancellationToken),
                hausBus.PublishAsync(RoutableEvent.FromEvent(new DiscoveryStartedEvent()), cancellationToken)
            )
            .ConfigureAwait(false);
    }
}
