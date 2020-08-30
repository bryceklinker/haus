using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Identity.Models.Clients;
using Haus.Identity.Models.Settings;
using Haus.Portal.Web.Settings.Entities;
using Haus.Portal.Web.Settings.Queries;
using Haus.ServiceBus.Publish;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Haus.Portal.Web.Settings
{
    [AllowAnonymous]
    [Route("settings")]
    public class SettingsController : Controller
    {
        private readonly ICqrsBus _cqrsBus;
        private readonly IHausServiceBusPublisher _publisher;

        public SettingsController(ICqrsBus cqrsBus, IHausServiceBusPublisher publisher)
        {
            _cqrsBus = cqrsBus;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var settings = await _cqrsBus.ExecuteQuery(new GetSettingsQuery());
            if (string.IsNullOrWhiteSpace(settings?.Authority))
            {
                await _publisher.PublishAsync(new RequestAuthoritySettingsPayload());
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(settings?.ClientId))
            {
                await _publisher.PublishAsync(new CreateClientPayload(AuthSettings.ClientName));
                return NotFound();
            }

            return Ok(settings);
        }
    }
}