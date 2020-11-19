using Haus.Web.Host.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.Settings
{
    [ApiController]
    [Route("settings")]
    public class SettingsController : Controller
    {
        private readonly IOptions<AuthOptions> _authOptions;

        private string Domain => _authOptions.Value.Domain;
        private string ClientId => _authOptions.Value.ClientId;
        private string Audience => _authOptions.Value.Audience;

        public SettingsController(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(new ClientSettingsModel
            {
                Auth = new ClientAuthSettingsModel
                {
                    Domain = Domain,
                    ClientId = ClientId,
                    Audience = Audience
                }
            });
        }
    }
}