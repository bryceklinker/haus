using Haus.Core;
using Haus.Core.Models.ClientSettings;
using Haus.Web.Host.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.ClientSettings;

[ApiController]
[Route("client-settings")]
public class ClientSettingsController(IOptions<AuthOptions> authOptions) : Controller
{
    private string Domain => authOptions.Value.Domain;
    private string ClientId => authOptions.Value.ClientId;
    private string Audience => authOptions.Value.Audience;
    private string? Version => typeof(Startup).Assembly.GetName().Version?.ToSemanticVersion();

    [AllowAnonymous]
    public IActionResult Get()
    {
        var authSettings = new AuthSettingsModel(Domain, ClientId, Audience);
        var clientSettings = new ClientSettingsModel(Version, authSettings);
        return Ok(clientSettings);
    }
}
