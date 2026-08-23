using System.Threading.Tasks;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Queries;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Devices;

[Route("api/devices")]
public class DevicesController(IHausBus hausBus) : HausBusController(hausBus)
{
    [HttpGet("")]
    public async Task<IActionResult> Get([FromQuery] string? externalId = null)
    {
        return await QueryAsync(new GetDevicesQuery(externalId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id)
    {
        return await QueryAsync(new GetDeviceByIdQuery(id));
    }

    [HttpPut("{id}/lighting")]
    public async Task<IActionResult> ChangeLighting([FromRoute] long id, [FromBody] LightingModel model)
    {
        return await CommandAsync(new ChangeDeviceLightingCommand(id, model));
    }

    [HttpPut("{id}/lighting-constraints")]
    public async Task<IActionResult> ChangeLightingConstraints(
        [FromRoute] long id,
        [FromBody] LightingConstraintsModel model
    )
    {
        return await CommandAsync(new ChangeDeviceLightingConstraintsCommand(id, model));
    }

    [HttpPost("{id}/turn-off")]
    public async Task<IActionResult> TurnOff([FromRoute] long id)
    {
        return await CommandAsync(new TurnDeviceOffCommand(id));
    }

    [HttpPost("{id}/turn-on")]
    public async Task<IActionResult> TurnOn([FromRoute] long id)
    {
        return await CommandAsync(new TurnDeviceOnCommand(id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] DeviceModel model)
    {
        return await CommandAsync(new UpdateDeviceCommand(model with { Id = id }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        return await CommandAsync(new DeleteDeviceCommand(id));
    }
}
