using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Devices.Queries;

public record GetDeviceByIdQuery(long Id) : GetByIdQuery<DeviceModel>(Id);

internal class GetDeviceByIdQueryHandler : IQueryHandler<GetDeviceByIdQuery, DeviceModel>
{
    private readonly HausDbContext _context;

    public GetDeviceByIdQueryHandler(HausDbContext context)
    {
        _context = context;
    }

    public async Task<DeviceModel> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken = default)
    {
        return await _context.QueryAll<DeviceEntity>()
            .Where(d => d.Id == request.Id)
            .Select(DeviceEntity.ToModelExpression)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}