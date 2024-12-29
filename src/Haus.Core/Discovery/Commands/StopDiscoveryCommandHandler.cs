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

internal class StopDiscoveryCommandHandler : ICommandHandler<StopDiscoveryCommand>
{
    private readonly HausDbContext _context;
    private readonly IHausBus _hausBus;

    public StopDiscoveryCommandHandler(IHausBus hausBus, HausDbContext context)
    {
        _hausBus = hausBus;
        _context = context;
    }

    public async Task Handle(StopDiscoveryCommand request, CancellationToken cancellationToken)
    {
        var model = new StopDiscoveryModel();

        var discovery = await _context.GetDiscoveryEntityAsync(cancellationToken).ConfigureAwait(false);
        discovery.Stop();
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await Task.WhenAll(
            _hausBus.PublishAsync(RoutableCommand.FromEvent(model), cancellationToken),
            _hausBus.PublishAsync(RoutableEvent.FromEvent(new DiscoveryStoppedEvent()), cancellationToken)
        ).ConfigureAwait(false);
    }
}