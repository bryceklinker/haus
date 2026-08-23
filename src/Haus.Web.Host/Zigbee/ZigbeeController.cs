using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Haus.Core.Zigbee.Queries;
using Haus.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Zigbee;

[Authorize]
[ApiController]
[Route("api/zigbee")]
public class ZigbeeController(IHausBus hausBus) : Controller
{
    [HttpGet("status")]
    public async Task<ZigbeeConnectionStatusModel> GetStatus()
    {
        return await hausBus.ExecuteQueryAsync(new GetZigbeeConnectionStatusQuery());
    }

    [HttpGet("activity")]
    public async Task<ListResult<ZigbeeActivityEntryModel>> GetActivity()
    {
        return await hausBus.ExecuteQueryAsync(new GetRecentZigbeeActivityQuery());
    }

    [HttpGet("devices")]
    public async Task<ListResult<ZigbeeKnownDeviceModel>> GetDevices()
    {
        return await hausBus.ExecuteQueryAsync(new GetKnownZigbeeDevicesQuery());
    }
}
