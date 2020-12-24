using System;
using System.Linq;
using Haus.Core.Models.Devices;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Device.Simulator.Devices
{
    [ApiController]
    [Route("api/deviceTypes")]
    public class DeviceTypesController : Controller
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(Enum.GetNames<DeviceType>().OrderBy(s => s));
        }
    }
}