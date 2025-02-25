using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host.Health;

[Authorize]
public class HealthHub(ILastKnownHealthCache lastKnownHealthCache) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("OnHealth", lastKnownHealthCache.GetLatestReport());
    }
}
