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

public class GetAllHealthChecksQueryHandler(HausDbContext context)
    : IQueryHandler<GetAllHealthChecksQuery, ListResult<HausHealthCheckModel>>
{
    public async Task<ListResult<HausHealthCheckModel>> Handle(GetAllHealthChecksQuery request,
        CancellationToken cancellationToken)
    {
        return await context.QueryAll<HealthCheckEntity>()
            .Select(HealthCheckEntity.ToModelExpression)
            .ToListResultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}