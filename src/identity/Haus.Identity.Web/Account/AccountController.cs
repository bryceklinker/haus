using System.Threading.Tasks;
using Haus.Identity.Core.Accounts;
using Haus.Identity.Core.Accounts.Models;
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
            var request = new LoginRequest
            {
                ReturnUrl = returnUrl
            };
            return View(request);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginRequest request, [FromQuery] string returnUrl = null)
        {
            request.ReturnUrl = returnUrl;

            var signInResult = await _signInManager.PasswordSignInAsync(request);
            if (signInResult.Succeeded)
                return RedirectToAction("Index", "Home");

            return View(request);
        }
    }
}