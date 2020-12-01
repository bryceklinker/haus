using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Discovery;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public class StartDiscoveryCommand : ICommand
    {
        
    }
    
    public class StartDiscoveryCommandHandler : AsyncRequestHandler<StartDiscoveryCommand>, ICommandHandler<StartDiscoveryCommand>
    {
        private readonly IHausBus _hausBus;

        public StartDiscoveryCommandHandler(IHausBus hausBus)
        {
            _hausBus = hausBus;
        }

        protected override async Task Handle(StartDiscoveryCommand request, CancellationToken cancellationToken)
        {
            var model = new StartDiscoveryModel();
            await _hausBus.PublishAsync(new RoutableCommand(model.AsHausCommand()), cancellationToken);
        }
    }
}