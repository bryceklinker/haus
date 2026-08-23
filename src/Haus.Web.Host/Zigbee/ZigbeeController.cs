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
    public Task<IActionResult> GetStatus()
    {
        return QueryAsync(new GetZigbeeConnectionStatusQuery());
    }

    [HttpGet("activity")]
    public Task<IActionResult> GetActivity()
    {
        return QueryAsync(new GetRecentZigbeeActivityQuery());
    }

    [HttpGet("devices")]
    public Task<IActionResult> GetDevices()
    {
        return QueryAsync(new GetKnownZigbeeDevicesQuery());
    }
}
