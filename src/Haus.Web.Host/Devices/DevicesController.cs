using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Queries;
using Haus.Core.Devices.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Devices
{
    [Authorize]
    [ApiController]
    [Route("api/devices")]
    public class DevicesController : Controller
    {
        private readonly IHausBus _hausBus;

        public DevicesController(IHausBus hausBus)
        {
            _hausBus = hausBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var result = await _hausBus.ExecuteQueryAsync(new GetDevicesQuery());
            return Ok(result);
        }
    }
}