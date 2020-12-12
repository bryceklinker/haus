using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Discovery;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public class SyncDiscoveryCommand : ICommand
    {
    }

    internal class SyncDiscoveryCommandHandler : AsyncRequestHandler<SyncDiscoveryCommand>, ICommandHandler<SyncDiscoveryCommand>
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
}