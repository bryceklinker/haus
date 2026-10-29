using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Diagnostics;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Models;
using Haus.Zigbee.Models.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Zigbee.Host.Zigbee;

// Read-only diagnostics for Haus.Web.Host to query the Zigbee protocol layer directly. Status and
// activity come from the in-memory diagnostics store; known devices come from this host's own
// database. Intentionally unauthenticated -- it is reachable only on the internal service network.
[ApiController]
[Route("api/zigbee")]
public class ZigbeeDiagnosticsController(IZigbeeDiagnosticsStore diagnosticsStore, IKnownZigbeeDeviceStore deviceStore)
    : ControllerBase
{
    [HttpGet("status")]
    public ZigbeeConnectionStatusModel GetStatus()
    {
        return diagnosticsStore.Current.ConnectionStatus;
    }

    [HttpGet("activity")]
    public IReadOnlyList<ZigbeeActivityEntryModel> GetActivity()
    {
        return diagnosticsStore.Current.RecentActivity;
    }

    [HttpGet("devices")]
    public async Task<IReadOnlyList<ZigbeeKnownDeviceModel>> GetDevices(CancellationToken token)
    {
        var devices = await deviceStore.GetAllAsync(token);
        return devices.Select(ToModel).ToList();
    }

    private static ZigbeeKnownDeviceModel ToModel(KnownZigbeeDeviceRecord record)
    {
        return new ZigbeeKnownDeviceModel(
            record.ExternalId,
            record.NetworkAddress,
            record.Endpoints.Select(ToEndpointModel).ToList(),
            record.IsSleepy
        );
    }

    private static ZigbeeEndpointModel ToEndpointModel(ZigbeeEndpoint endpoint)
    {
        return new ZigbeeEndpointModel(
            endpoint.EndpointId,
            endpoint.ProfileId,
            endpoint.DeviceId,
            endpoint.InClusters,
            endpoint.OutClusters
        );
    }
}
