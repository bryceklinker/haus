using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.State;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host.DeviceSimulator;

[Authorize]
public class DeviceSimulatorHub : Hub
{
    private readonly IDeviceSimulatorStore _store;

    public DeviceSimulatorHub(IDeviceSimulatorStore store)
    {
        _store = store;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("OnState", _store.Current.ToModel());
    }
}