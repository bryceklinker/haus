using System.Threading.Tasks;
using Haus.Identity.Web.Users.Entities;
using Haus.Identity.Web.Users.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Identity.Web.Users
{
    [AllowAnonymous]
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly SignInManager<HausUser> _signInManager;

        public UsersController(SignInManager<HausUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] string returnUrl = null)
        {
            return View("Login", new LoginViewModel
            {
                ReturntUrl = returnUrl
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel viewModel, [FromQuery] string returnUrl = null)
        {
            if (await viewModel.Login(_signInManager))
                return Redirect(returnUrl);
            return View("Login", new LoginViewModel
            {
                ReturntUrl = returnUrl,
                Username = viewModel.Username
            });
        }
    }
}