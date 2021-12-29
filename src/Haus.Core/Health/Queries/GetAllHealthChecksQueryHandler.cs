using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Health.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Health;
using Haus.Cqrs.Queries;

namespace Haus.Core.Health.Queries;

public record GetAllHealthChecksQuery : IQuery<ListResult<HausHealthCheckModel>>;

public class GetAllHealthChecksQueryHandler : IQueryHandler<GetAllHealthChecksQuery, ListResult<HausHealthCheckModel>>
{
    private readonly HausDbContext _context;

    public GetAllHealthChecksQueryHandler(HausDbContext context)
    {
        _context = context;
    }

    public async Task<ListResult<HausHealthCheckModel>> Handle(GetAllHealthChecksQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.QueryAll<HealthCheckEntity>()
            .Select(HealthCheckEntity.ToModelExpression)
            .ToListResultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}