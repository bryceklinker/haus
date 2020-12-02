using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Queries;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Queries;
using Haus.Core.Models.Devices;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Devices
{
    [Authorize]
    [ApiController]
    [Route("api/devices")]
    public class DevicesController : HausBusController
    {
        public DevicesController(IHausBus hausBus)
            : base(hausBus)
        {
        }

        [HttpGet("")]
        public Task<IActionResult> Get([FromQuery] string externalId = null)
        {
            return QueryAsync(new GetDevicesQuery(externalId));
        }

        [HttpGet("{id}")]
        public Task<IActionResult> GetById([FromRoute] long id)
        {
            return QueryAsync(new GetDeviceByIdQuery(id));
        }
        
        [HttpPut("{id}")]
        public Task<IActionResult> Update([FromRoute] long id, [FromBody] DeviceModel model)
        {
            return CommandAsync(new UpdateDeviceCommand(id, model));
        }

        [HttpPost("start-discovery")]
        public Task<IActionResult> StartDiscovery()
        {
            return CommandAsync(new StartDiscoveryCommand());
        }

        [HttpPost("stop-discovery")]
        public Task<IActionResult> StopDiscovery()
        {
            return CommandAsync(new StopDiscoveryCommand());
        }
    }
}