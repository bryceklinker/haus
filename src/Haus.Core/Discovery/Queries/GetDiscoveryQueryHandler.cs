using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Discovery.Entities;
using Haus.Core.Models.Discovery;
using Haus.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Discovery.Queries;

public class GetDiscoveryQuery : IQuery<DiscoveryModel>
{
}

internal class GetDiscoveryQueryHandler : IQueryHandler<GetDiscoveryQuery, DiscoveryModel>
{
    private readonly HausDbContext _context;

    public GetDiscoveryQueryHandler(HausDbContext context)
    {
        _context = context;
    }

    public Task<DiscoveryModel> Handle(GetDiscoveryQuery request, CancellationToken cancellationToken)
    {
        return _context.Set<DiscoveryEntity>()
            .Select(DiscoveryEntity.ToModelExpression)
            .SingleOrDefaultAsync(cancellationToken);
    }
}