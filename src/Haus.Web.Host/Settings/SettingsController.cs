using Haus.Web.Host.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.Settings
{
    [Route("settings")]
    public class SettingsController : Controller
    {
        private readonly IOptions<AuthOptions> _authOptions;

        private string Domain => _authOptions.Value.Domain;
        private string ClientId => _authOptions.Value.ClientId;

        public SettingsController(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        public IActionResult Get()
        {
            return Ok(new ClientSettingsModel
            {
                Auth = new ClientAuthSettingsModel
                {
                    Domain = Domain,
                    ClientId = ClientId
                }
            });
        }
    }
}