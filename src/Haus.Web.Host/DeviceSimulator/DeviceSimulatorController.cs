using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.Commands;
using Haus.Core.Models.DeviceSimulator;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.DeviceSimulator;

[Route("api/device-simulator")]
public class DeviceSimulatorController(IHausBus hausBus) : HausBusController(hausBus)
{
    [HttpPost("devices")]
    public async Task<IActionResult> AddDevice([FromBody] SimulatedDeviceModel model)
    {
        return await CommandAsync(new CreateSimulatedDeviceCommand(model));
    }

    [HttpPost("devices/{id}/trigger-occupancy-change")]
    public async Task<IActionResult> TriggerOccupancyChange([FromRoute] string id)
    {
        return await CommandAsync(new TriggerOccupancyChangedCommand(id));
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        return await CommandAsync(new ResetDeviceSimulatorCommand());
    }
}
