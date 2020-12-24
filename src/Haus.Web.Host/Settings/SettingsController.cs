using Haus.Web.Host.Auth;
using Haus.Web.Host.DeviceSimulator;
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
        private readonly IOptions<DeviceSimulatorOptions> _deviceSimulatorOptions;

        private string Domain => _authOptions.Value.Domain;
        private string ClientId => _authOptions.Value.ClientId;
        private string Audience => _authOptions.Value.Audience;
        private string DeviceSimulatorUrl => _deviceSimulatorOptions.Value.Url;
        private bool IsDeviceSimulatorEnabled => _deviceSimulatorOptions.Value.IsEnabled;

        public SettingsController(IOptions<AuthOptions> authOptions, IOptions<DeviceSimulatorOptions> deviceSimulatorOptions)
        {
            _authOptions = authOptions;
            _deviceSimulatorOptions = deviceSimulatorOptions;
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
                },
                DeviceSimulator = new DeviceSimulatorSettingsModel
                {
                    Url = DeviceSimulatorUrl,
                    IsEnabled = IsDeviceSimulatorEnabled
                }
            });
        }
    }
}