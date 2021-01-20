using Haus.Core.Models.ClientSettings;
using Haus.Web.Host.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.ClientSettings
{
    [ApiController]
    [Route("client-settings")]
    public class ClientSettingsController : Controller
    {
        private readonly IOptions<AuthOptions> _authOptions;

        private string Domain => _authOptions.Value.Domain;
        private string ClientId => _authOptions.Value.ClientId;
        private string Audience => _authOptions.Value.Audience;
        private string Version => typeof(Startup).Assembly.GetName().Version?.ToString(3);
        
        public ClientSettingsController(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        [AllowAnonymous]
        public IActionResult Get()
        {
            var authSettings = new AuthSettingsModel(Domain, ClientId, Audience);
            var clientSettings = new ClientSettingsModel(Version, authSettings);
            return Ok(clientSettings);
        }
    }
}