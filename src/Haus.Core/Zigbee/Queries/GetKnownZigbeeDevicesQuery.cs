using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Haus.Core.Zigbee.State;
using Haus.Cqrs.Queries;

namespace Haus.Core.Zigbee.Queries;

public record GetKnownZigbeeDevicesQuery : IQuery<ListResult<ZigbeeKnownDeviceModel>>;

public class GetKnownZigbeeDevicesQueryHandler(IZigbeeState state)
    : IQueryHandler<GetKnownZigbeeDevicesQuery, ListResult<ZigbeeKnownDeviceModel>>
{
    public Task<ListResult<ZigbeeKnownDeviceModel>> Handle(
        GetKnownZigbeeDevicesQuery request,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(new ListResult<ZigbeeKnownDeviceModel>(state.KnownDevices.Values.ToArray()));
    }
}
