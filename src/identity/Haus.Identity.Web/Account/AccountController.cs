using System.Threading.Tasks;
using Haus.Identity.Core.Accounts;
using Haus.Identity.Core.Accounts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Identity.Web.Account
{
    [Route("api/account")]
    public class AccountsController : Controller
    {
        private readonly SignInManager<HausUser> _signInManager;

        public AccountsController(SignInManager<HausUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _signInManager.PasswordSignInAsync(loginRequest);

            if (result.Succeeded)
                return Ok();
            return Unauthorized();
        }
    }
}