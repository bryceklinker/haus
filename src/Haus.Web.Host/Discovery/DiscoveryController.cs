using System.Threading.Tasks;
using Haus.Core.Discovery.Commands;
using Haus.Core.Discovery.Queries;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Discovery;

[Route("api/discovery")]
public class DiscoveryController : HausBusController
{
    public DiscoveryController(IHausBus hausBus) : base(hausBus)
    {
    }

    [HttpGet("state")]
    public Task<IActionResult> GetDiscovery()
    {
        return QueryAsync(new GetDiscoveryQuery());
    }

    [HttpPost("start")]
    public Task<IActionResult> StartDiscovery()
    {
        return CommandAsync(new StartDiscoveryCommand());
    }

    [HttpPost("stop")]
    public Task<IActionResult> StopDiscovery()
    {
        return CommandAsync(new StopDiscoveryCommand());
    }

    [HttpPost("sync")]
    public Task<IActionResult> Sync()
    {
        return CommandAsync(new SyncDiscoveryCommand());
    }
}