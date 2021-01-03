using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Discovery;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public record StopDiscoveryCommand : ICommand;

    internal class StopDiscoveryCommandHandler : AsyncRequestHandler<StopDiscoveryCommand>, ICommandHandler<StopDiscoveryCommand>
    {
        private readonly IHausBus _hausBus;

        public StopDiscoveryCommandHandler(IHausBus hausBus)
        {
            _hausBus = hausBus;
        }

        protected override Task Handle(StopDiscoveryCommand request, CancellationToken cancellationToken)
        {
            var model = new StopDiscoveryModel();
            return _hausBus.PublishAsync(RoutableCommand.FromEvent(model), cancellationToken);
        }
    }
}