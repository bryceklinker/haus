using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Discovery.Entities;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Discovery.Commands;

public class InitializeDiscoveryCommand : ICommand
{
}

public class InitializeDiscoveryCommandHandler : ICommandHandler<InitializeDiscoveryCommand>
{
    private readonly HausDbContext _context;

    public InitializeDiscoveryCommandHandler(HausDbContext context)
    {
        _context = context;
    }

    public async Task Handle(InitializeDiscoveryCommand request, CancellationToken cancellationToken)
    {
        var discovery = await _context.GetDiscoveryEntityAsync(cancellationToken);
        if (discovery != null)
            return;

        _context.Add(new DiscoveryEntity());
        await _context.SaveChangesAsync(cancellationToken);
    }
}