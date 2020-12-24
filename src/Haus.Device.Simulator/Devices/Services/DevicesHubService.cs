using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Device.Simulator.Devices.Services
{
    public interface IDevicesHubService
    {
        Task PublishStateAsync(IDevicesState state);
    }
    
    public class DevicesHubService : IDevicesHubService
    {
        private readonly IHubContext<DevicesHub> _hubContext;

        public DevicesHubService(IHubContext<DevicesHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishStateAsync(IDevicesState state)
        {
            await _hubContext.Clients.All.SendAsync("OnStateChange", state).ConfigureAwait(false);
        }
    }
}