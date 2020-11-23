using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;

namespace Haus.Core.Devices.Queries
{
    public class GetDevicesQuery : IQuery<ListResult<DeviceModel>>
    {
        
    }
    
    public class GetDevicesQueryHandler : IQueryHandler<GetDevicesQuery, ListResult<DeviceModel>>
    {
        private readonly HausDbContext _context;

        public GetDevicesQueryHandler(HausDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<DeviceModel>> Handle(GetDevicesQuery request, CancellationToken cancellationToken = default)
        {
            return await _context.Set<DeviceEntity>()
                .Select(DeviceEntity.ToModel())
                .ToListResultAsync()
                .ConfigureAwait(false);
        }
    }
}