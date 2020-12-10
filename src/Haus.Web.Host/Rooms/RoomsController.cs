using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Queries;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Rooms
{
    [Route("api/rooms")]
    public class RoomsController : HausBusController
    {
        public RoomsController(IHausBus hausBus) : base(hausBus)
        {
        }

        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            return QueryAsync(new GetRoomsQuery());
        }

        [HttpGet("{id}", Name = "GetRoomById")]
        public Task<IActionResult> GetById([FromRoute] long id)
        {
            return QueryAsync(new GetRoomByIdQuery(id));
        }

        [HttpPost]
        public Task<IActionResult> Create([FromBody] RoomModel model)
        {
            return CreateCommandAsync(new CreateRoomCommand(model), "GetRoomById", m => new {id = m.Id});
        }

        [HttpPut("{id}")]
        public Task<IActionResult> Update([FromRoute] long id, [FromBody] RoomModel model)
        {
            return CommandAsync(new UpdateRoomCommand(id, model));
        }

        [HttpPost("{id}/add-devices")]
        public Task<IActionResult> AddDevicesToRoom([FromRoute] long id, [FromBody] long[] deviceIds)
        {
            return CommandAsync(new AddDevicesToRoomCommand(id, deviceIds));
        }

        [HttpGet("{id}/devices")]
        public Task<IActionResult> GetDevicesInRoom([FromRoute] long id)
        {
            return QueryAsync(new GetDevicesInRoomQuery(id));
        }

        [HttpPost("{id}/lighting")]
        public Task<IActionResult> ChangeLighting([FromRoute] long id, [FromBody] LightingModel model)
        {
            return CommandAsync(new ChangeRoomLightingCommand(id, model));
        }

        [HttpPost("{id}/turn-off")]
        public Task<IActionResult> TurnOff([FromRoute] long id)
        {
            return CommandAsync(new TurnRoomOffCommand(id));
        }
        
        [HttpPost("{id}/turn-on")]
        public Task<IActionResult> TurnOn([FromRoute] long id)
        {
            return CommandAsync(new TurnRoomOnCommand(id));
        }
    }
}