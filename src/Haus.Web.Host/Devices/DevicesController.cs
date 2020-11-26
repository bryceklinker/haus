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
        public async Task<IActionResult> Get([FromQuery] string externalId = null)
        {
            return await QueryAsync(new GetDevicesQuery(externalId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] long id)
        {
            return await QueryAsync(new GetDeviceByIdQuery(id));
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] DeviceModel model)
        {
            return await CommandAsync(new UpdateDeviceCommand(id, model));
        }
    }
}