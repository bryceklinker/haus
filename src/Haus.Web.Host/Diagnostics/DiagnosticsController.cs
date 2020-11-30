using System.Threading.Tasks;
using Haus.Core.Models.Diagnostics;
using Haus.Web.Host.Common.Mqtt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Diagnostics
{
    [Authorize]
    [ApiController]
    [Route("api/diagnostics")]
    public class DiagnosticsController : Controller
    {
        private readonly IHausMqttClientFactory _hausMqttClientFactory;

        public DiagnosticsController(IHausMqttClientFactory hausMqttClientFactory)
        {
            _hausMqttClientFactory = hausMqttClientFactory;
        }

        [HttpPost("replay")]
        public async Task<IActionResult> Replay([FromBody] MqttDiagnosticsMessageModel model)
        {
            var client = await _hausMqttClientFactory.CreateClient();
            await client.PublishAsync(model.ToMqttMessage());
            return NoContent();
        }
    }
}