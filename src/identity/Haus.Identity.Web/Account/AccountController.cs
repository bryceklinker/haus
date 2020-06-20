using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Web.Account.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Identity.Web.Account
{
    [Authorize]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<HausUser> _signInManager;

        public AccountController(SignInManager<HausUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login([FromQuery] string returnUrl = null)
        {
            var request = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(request);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginViewModel viewModel, [FromQuery] string returnUrl = null)
        {
            viewModel.ReturnUrl = returnUrl;

            var signInResult = await _signInManager.PasswordSignInAsync(viewModel);
            if (signInResult.Succeeded)
                return Redirect(returnUrl);

            return View(viewModel);
        }
    }
}