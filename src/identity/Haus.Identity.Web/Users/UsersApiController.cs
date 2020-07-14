using System.Net;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Identity.Core.Users.GetUsers;
using Haus.Identity.Web.Users.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Identity.Web.Users
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : Controller
    {
        private readonly IMessageBus _messageBus;

        public UsersApiController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetUsersQuery();
            return Ok(await _messageBus.ExecuteQuery(query));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserViewModel viewModel)
        {
            var result = await _messageBus.ExecuteCommand(viewModel.ToCommand());
            if (result.WasSuccessful)
                return StatusCode((int) HttpStatusCode.Created, result);

            return BadRequest(result);
        } 
    }
}