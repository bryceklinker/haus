using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.Devices.Events;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public record StartDiscoveryCommand : ICommand;

    internal class StartDiscoveryCommandHandler : AsyncRequestHandler<StartDiscoveryCommand>,
        ICommandHandler<StartDiscoveryCommand>
    {
        private readonly IHausBus _hausBus;

        public StartDiscoveryCommandHandler(IHausBus hausBus)
        {
            _hausBus = hausBus;
        }

        protected override Task Handle(StartDiscoveryCommand request, CancellationToken cancellationToken)
        {
            var model = new StartDiscoveryModel();
            return Task.WhenAll(
                _hausBus.PublishAsync(RoutableCommand.FromEvent(model), cancellationToken),
                _hausBus.PublishAsync(RoutableEvent.FromEvent(new DiscoveryStartedEvent()), cancellationToken)
            );
        }
    }
}