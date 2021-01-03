using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Cqrs.Queries;

namespace Haus.Core.Devices.Queries
{
    public record GetDeviceTypesQuery : IQuery<ListResult<DeviceType>>;

    public class GetDeviceTypesQueryHandler : IQueryHandler<GetDeviceTypesQuery, ListResult<DeviceType>>
    {
        public Task<ListResult<DeviceType>> Handle(GetDeviceTypesQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enum.GetValues<DeviceType>().ToListResult());
        }
    }
}