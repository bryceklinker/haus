using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.State;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host.DeviceSimulator;

[Authorize]
public class DeviceSimulatorHub(IDeviceSimulatorStore store) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("OnState", store.Current.ToModel());
    }
}