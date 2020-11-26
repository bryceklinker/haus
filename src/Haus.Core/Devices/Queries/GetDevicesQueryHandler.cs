using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Haus.Core.Common;
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
        private readonly IMapper _mapper;

        public GetDevicesQueryHandler(HausDbContext context, IMapper mapper = null)
        {
            _context = context;
            _mapper = mapper ?? DefaultMapperFactory.GetMapper();
        }

        public async Task<ListResult<DeviceModel>> Handle(GetDevicesQuery request, CancellationToken cancellationToken = default)
        {
            return await _context.Set<DeviceEntity>()
                .ProjectTo<DeviceModel>(_mapper.ConfigurationProvider)
                .ToListResultAsync()
                .ConfigureAwait(false);
        }
    }
}