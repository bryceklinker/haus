using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Device.Simulator.Devices.Services;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Device.Simulator.Devices
{
    [ApiController]
    [Route("api/devices")]
    public class DevicesController : Controller
    {
        private readonly IDevicesService _service;

        public DevicesController(IDevicesService service)
        {
            _service = service;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody] DeviceModel model)
        {
            await _service.AddDevice(model).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAllDevices());
        }

        [HttpPost("clear")]
        public IActionResult Clear()
        {
            _service.ClearState();
            return NoContent();
        }
    }
}