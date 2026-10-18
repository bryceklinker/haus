using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Zigbee.Host.Storage;

public interface IKnownZigbeeDeviceStore
{
    Task<IReadOnlyList<KnownZigbeeDeviceRecord>> GetAllAsync(CancellationToken token = default);

    Task UpsertNetworkAddressAsync(string externalId, ushort networkAddress, CancellationToken token = default);

    Task UpsertAsync(
        string externalId,
        ushort networkAddress,
        string endpointsJson,
        bool isSleepy,
        CancellationToken token = default
    );
}
