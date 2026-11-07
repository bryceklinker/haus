using System.Linq;
using System.Threading.Tasks;
using Haus.Api.Client.Zigbee;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HostModels = Haus.Zigbee.Models.Diagnostics;

namespace Haus.Web.Host.Zigbee;

[Authorize]
[ApiController]
[Route("api/zigbee")]
public class ZigbeeController(IZigbeeHostApiClient zigbeeHostApiClient) : Controller
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var status = await zigbeeHostApiClient.GetStatusAsync();
        if (status == null)
            return NotFound();
        return Ok(ToModel(status));
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity()
    {
        var activity = await zigbeeHostApiClient.GetActivityAsync();
        return Ok(new ListResult<ZigbeeActivityEntryModel>(activity.Select(ToModel).ToArray()));
    }

    [HttpGet("devices")]
    public async Task<IActionResult> GetDevices()
    {
        var devices = await zigbeeHostApiClient.GetDevicesAsync();
        return Ok(new ListResult<ZigbeeKnownDeviceModel>(devices.Select(ToModel).ToArray()));
    }

    private static ZigbeeConnectionStatusModel ToModel(HostModels.ZigbeeConnectionStatusModel model)
    {
        return new ZigbeeConnectionStatusModel(model.IsConnected, model.Reason, model.ChangedAt);
    }

    private static ZigbeeActivityEntryModel ToModel(HostModels.ZigbeeActivityEntryModel model)
    {
        return new ZigbeeActivityEntryModel(model.EventType, model.OccurredAt, model.Payload);
    }

    // The Zigbee.Host diagnostics store doesn't track manufacturer/model/last-seen -- those were
    // derived from ZCL discovery events the old Core mirror observed over MQTT. Left blank here;
    // reinstating them is tracked separately from this read-path cutover (issue #99/#100).
    private static ZigbeeKnownDeviceModel ToModel(HostModels.ZigbeeKnownDeviceModel model)
    {
        return new ZigbeeKnownDeviceModel(
            model.IeeeAddress,
            model.NetworkAddress,
            ManufacturerName: null,
            ModelIdentifier: null,
            model.Endpoints.Select(ToModel).ToList(),
            LastSeenAt: default,
            model.IsSleepy
        );
    }

    private static ZigbeeEndpointModel ToModel(HostModels.ZigbeeEndpointModel model)
    {
        return new ZigbeeEndpointModel(model.EndpointId, model.InClusters, model.OutClusters);
    }
}
