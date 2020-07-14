using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Haus.Web.Settings
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _config;

        public SettingsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult GetSettings()
        {
            return Ok(SettingsModel.FromConfiguration(_config));
        }
    }
}