using System.Threading.Tasks;
using Haus.Core.Zigbee.Queries;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Zigbee;

[Route("api/zigbee")]
public class ZigbeeController(IHausBus hausBus) : HausBusController(hausBus)
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        return await QueryAsync(new GetZigbeeConnectionStatusQuery());
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity()
    {
        return await QueryAsync(new GetRecentZigbeeActivityQuery());
    }

    [HttpGet("devices")]
    public async Task<IActionResult> GetDevices()
    {
        return await QueryAsync(new GetKnownZigbeeDevicesQuery());
    }
}
