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

    [HttpPut("{id}/lighting")]
    public Task<IActionResult> ChangeLighting([FromRoute] long id, [FromBody] LightingModel model)
    {
        return CommandAsync(new ChangeDeviceLightingCommand(id, model));
    }

    [HttpPut("{id}/lighting-constraints")]
    public Task<IActionResult> ChangeLightingConstraints([FromRoute] long id,
        [FromBody] LightingConstraintsModel model)
    {
        return CommandAsync(new ChangeDeviceLightingConstraintsCommand(id, model));
    }

    [HttpPost("{id}/turn-off")]
    public Task<IActionResult> TurnOff([FromRoute] long id)
    {
        return CommandAsync(new TurnDeviceOffCommand(id));
    }

    [HttpPost("{id}/turn-on")]
    public Task<IActionResult> TurnOn([FromRoute] long id)
    {
        return CommandAsync(new TurnDeviceOnCommand(id));
    }

    [HttpPut("{id}")]
    public Task<IActionResult> Update([FromRoute] long id, [FromBody] DeviceModel model)
    {
        return CommandAsync(new UpdateDeviceCommand(model with { Id = id }));
    }
}