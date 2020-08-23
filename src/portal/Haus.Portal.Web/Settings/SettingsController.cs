using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Haus.Portal.Web.Settings
{
    [AllowAnonymous]
    [Route("settings")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _config;

        public SettingsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(SettingsModel.FromConfig(_config));
        }
    }
}