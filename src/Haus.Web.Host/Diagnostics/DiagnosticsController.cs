using System.Threading.Tasks;
using Haus.Core.Models.Diagnostics;
using Haus.Mqtt.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Diagnostics;

[Authorize]
[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsController(IHausMqttClientFactory hausMqttClientFactory) : Controller
{
    [HttpPost("replay")]
    public async Task<IActionResult> Replay([FromBody] MqttDiagnosticsMessageModel model)
    {
        var client = await hausMqttClientFactory.CreateClient();
        await client.PublishAsync(model.ToMqttMessage());
        return NoContent();
    }
}
