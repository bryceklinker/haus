using System.Threading.Tasks;
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
        private readonly IQueryBus _queryBus;

        public DevicesController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var result = await _queryBus.Execute(new GetDevicesQuery());
            return Ok(result);
        }
    }
}