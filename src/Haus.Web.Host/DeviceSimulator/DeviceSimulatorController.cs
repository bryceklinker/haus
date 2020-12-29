using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.Commands;
using Haus.Core.Models.DeviceSimulator;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.DeviceSimulator
{
    [Route("api/device-simulator")]
    public class DeviceSimulatorController : HausBusController
    {
        public DeviceSimulatorController(IHausBus hausBus) 
            : base(hausBus)
        {
        }

        [HttpPost("devices")]
        public Task<IActionResult> AddDevice([FromBody] CreateSimulatedDeviceModel model)
        {
            return CommandAsync(new CreateSimulatedDeviceCommand(model));
        }

        [HttpPost("reset")]
        public Task<IActionResult> Reset()
        {
            return CommandAsync(new ResetDeviceSimulatorCommand());
        }
    }
}