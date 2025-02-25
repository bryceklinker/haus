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

internal class GetDeviceByIdQueryHandler(HausDbContext context) : IQueryHandler<GetDeviceByIdQuery, DeviceModel>
{
    public async Task<DeviceModel> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken = default)
    {
        return await context
            .QueryAll<DeviceEntity>()
            .Where(d => d.Id == request.Id)
            .Select(DeviceEntity.ToModelExpression)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
