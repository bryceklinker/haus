using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Cqrs.Queries;

namespace Haus.Core.Devices.Queries
{
    public record GetDevicesQuery(string ExternalId = null) : IQuery<ListResult<DeviceModel>>
    {
        public bool HasExternalId => !string.IsNullOrWhiteSpace(ExternalId);
    }

    internal class GetDevicesQueryHandler : IQueryHandler<GetDevicesQuery, ListResult<DeviceModel>>
    {
        private readonly HausDbContext _context;

        public GetDevicesQueryHandler(HausDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<DeviceModel>> Handle(GetDevicesQuery request, CancellationToken cancellationToken = default)
        {
            var query = _context.QueryAll<DeviceEntity>();

            if (request.HasExternalId) 
                query = query.Where(d => d.ExternalId == request.ExternalId);
            
            return await query
                .Select(DeviceEntity.ToModelExpression)
                .ToListResultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}