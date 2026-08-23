using System.Threading.Tasks;
using Haus.Core.Discovery.Commands;
using Haus.Core.Discovery.Queries;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Discovery;

[Route("api/discovery")]
public class DiscoveryController(IHausBus hausBus) : HausBusController(hausBus)
{
    [HttpGet("state")]
    public async Task<IActionResult> GetDiscovery()
    {
        return await QueryAsync(new GetDiscoveryQuery());
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartDiscovery()
    {
        return await CommandAsync(new StartDiscoveryCommand());
    }

    [HttpPost("stop")]
    public async Task<IActionResult> StopDiscovery()
    {
        return await CommandAsync(new StopDiscoveryCommand());
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        return await CommandAsync(new SyncDiscoveryCommand());
    }
}
