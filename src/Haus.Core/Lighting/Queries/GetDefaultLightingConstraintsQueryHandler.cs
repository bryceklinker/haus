using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Lighting;
using Haus.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Lighting.Queries
{
    public record GetDefaultLightingConstraintsQuery : IQuery<LightingConstraintsModel>;

    internal class GetDefaultLightingConstraintsQueryHandler : IQueryHandler<GetDefaultLightingConstraintsQuery, LightingConstraintsModel>
    {
        private readonly HausDbContext _context;

        public GetDefaultLightingConstraintsQueryHandler(HausDbContext context)
        {
            _context = context;
        }

        public Task<LightingConstraintsModel> Handle(GetDefaultLightingConstraintsQuery request, CancellationToken cancellationToken)
        {
            return _context.QueryAll<DefaultLightingConstraintsEntity>()
                .Select(c => c.Constraints)
                .Select(LightingConstraintsEntity.ToModelExpression)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}