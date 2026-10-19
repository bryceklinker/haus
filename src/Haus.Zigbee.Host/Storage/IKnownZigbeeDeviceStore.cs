using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Zigbee.Host.Storage;

public interface IKnownZigbeeDeviceStore
{
    Task<IReadOnlyList<KnownZigbeeDeviceRecord>> GetAllAsync(CancellationToken token = default);

    Task UpsertNetworkAddressAsync(UpsertNetworkAddressArgs args, CancellationToken token = default);

    Task UpsertAsync(UpsertKnownZigbeeDeviceArgs args, CancellationToken token = default);
}
