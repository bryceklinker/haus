using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Discovery;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Discovery.Commands
{
    public record StartDiscoveryCommand : ICommand;

    internal class StartDiscoveryCommandHandler : AsyncRequestHandler<StartDiscoveryCommand>,
        ICommandHandler<StartDiscoveryCommand>
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public StartDiscoveryCommandHandler(IHausBus hausBus, HausDbContext context)
        {
            _hausBus = hausBus;
            _context = context;
        }

        protected override async Task Handle(StartDiscoveryCommand request, CancellationToken cancellationToken)
        {
            var model = new StartDiscoveryModel();
            
            var discovery = await _context.GetDiscoveryEntityAsync(cancellationToken).ConfigureAwait(false);
            discovery.Start();
            await _context.SaveChangesAsync(cancellationToken);
            
            await Task.WhenAll(
                _hausBus.PublishAsync(RoutableCommand.FromEvent(model), cancellationToken),
                _hausBus.PublishAsync(RoutableEvent.FromEvent(new DiscoveryStartedEvent()), cancellationToken)
            ).ConfigureAwait(false);
        }
    }
}