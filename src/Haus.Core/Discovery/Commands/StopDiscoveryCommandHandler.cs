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

public record StopDiscoveryCommand : ICommand;

internal class StopDiscoveryCommandHandler(IHausBus hausBus, HausDbContext context)
    : ICommandHandler<StopDiscoveryCommand>
{
    public async Task Handle(StopDiscoveryCommand request, CancellationToken cancellationToken)
    {
        var model = new StopDiscoveryModel();

        var discovery = await context.GetDiscoveryEntityAsync(cancellationToken).ConfigureAwait(false);
        discovery.Stop();
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await Task.WhenAll(
                hausBus.PublishAsync(RoutableCommand.FromEvent(model), cancellationToken),
                hausBus.PublishAsync(RoutableEvent.FromEvent(new DiscoveryStoppedEvent()), cancellationToken)
            )
            .ConfigureAwait(false);
    }
}
