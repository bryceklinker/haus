using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Discovery.Entities;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Discovery.Commands;

public class InitializeDiscoveryCommand : ICommand { }

public class InitializeDiscoveryCommandHandler(HausDbContext context) : ICommandHandler<InitializeDiscoveryCommand>
{
    public async Task Handle(InitializeDiscoveryCommand request, CancellationToken cancellationToken)
    {
        var discovery = await context.GetDiscoveryEntityAsync(cancellationToken);
        if (discovery != null)
            return;

        context.Add(new DiscoveryEntity());
        await context.SaveChangesAsync(cancellationToken);
    }
}
