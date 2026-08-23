using System.Threading.Tasks;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Queries;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Rooms;

[Route("api/rooms")]
public class RoomsController(IHausBus hausBus) : HausBusController(hausBus)
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return await QueryAsync(new GetRoomsQuery());
    }

    [HttpGet("{id}", Name = "GetRoomById")]
    public async Task<IActionResult> GetById([FromRoute] long id)
    {
        return await QueryAsync(new GetRoomByIdQuery(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoomModel model)
    {
        return await CreateCommandAsync(new CreateRoomCommand(model), "GetRoomById", m => new { id = m.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] RoomModel model)
    {
        return await CommandAsync(new UpdateRoomCommand(model with { Id = id }));
    }

    [HttpPost("{id}/add-devices")]
    public async Task<IActionResult> AddDevicesToRoom([FromRoute] long id, [FromBody] long[] deviceIds)
    {
        return await CommandAsync(new AssignDevicesToRoomCommand(id, deviceIds));
    }

    [HttpGet("{id}/devices")]
    public async Task<IActionResult> GetDevicesInRoom([FromRoute] long id)
    {
        return await QueryAsync(new GetDevicesInRoomQuery(id));
    }

    [HttpPut("{id}/lighting")]
    public async Task<IActionResult> ChangeLighting([FromRoute] long id, [FromBody] LightingModel model)
    {
        return await CommandAsync(new ChangeRoomLightingCommand(id, model));
    }

    [HttpPost("{id}/turn-off")]
    public async Task<IActionResult> TurnOff([FromRoute] long id)
    {
        return await CommandAsync(new TurnRoomOffCommand(id));
    }

    [HttpPost("{id}/turn-on")]
    public async Task<IActionResult> TurnOn([FromRoute] long id)
    {
        return await CommandAsync(new TurnRoomOnCommand(id));
    }
}
