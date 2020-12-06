using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Haus.Core.Common;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Devices.Queries
{
    public class GetDeviceByIdQuery : IQuery<DeviceModel>
    {
        public long Id { get; }

        public GetDeviceByIdQuery(long id)
        {
            Id = id;
        }
    }

    internal class GetDeviceByIdQueryHandler : IQueryHandler<GetDeviceByIdQuery, DeviceModel>
    {
        private readonly HausDbContext _context;
        private readonly IMapper _mapper;

        public GetDeviceByIdQueryHandler(HausDbContext context, IMapper mapper = null)
        {
            _context = context;
            _mapper = mapper ?? DefaultMapperFactory.GetMapper();
        }

        public async Task<DeviceModel> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken = default)
        {
            return await _context.GetAllReadOnly<DeviceEntity>()
                .Where(d => d.Id == request.Id)
                .ProjectTo<DeviceModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}