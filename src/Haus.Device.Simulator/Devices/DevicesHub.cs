using System.Threading.Tasks;
using Haus.Device.Simulator.Devices.Services;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Device.Simulator.Devices
{
    public class DevicesHub : Hub
    {
        private readonly IDevicesStore _store;
        private readonly IDevicesHubService _devicesHubService;

        public DevicesHub(IDevicesStore store, IDevicesHubService devicesHubService)
        {
            _store = store;
            _devicesHubService = devicesHubService;
        }

        public override async Task OnConnectedAsync()
        {
            await _devicesHubService.PublishStateAsync(_store.Current).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }
    }
}