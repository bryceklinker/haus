using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host.Health;

[Authorize]
public class HealthHub : Hub
{
    private readonly ILastKnownHealthCache _lastKnownHealthCache;

    public HealthHub(ILastKnownHealthCache lastKnownHealthCache)
    {
        _lastKnownHealthCache = lastKnownHealthCache;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("OnHealth", _lastKnownHealthCache.GetLatestReport());
    }
}