using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.DeviceSimulator;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.DeviceSimulator
{
    public interface IDeviceSimulatorApiClient
    {
        Task<HttpResponseMessage> TriggerOccupancyChange(string simulatorId);
        Task<HttpResponseMessage> AddSimulatedDeviceAsync(SimulatedDeviceModel model);
        Task<HttpResponseMessage> ResetDeviceSimulatorAsync();
    }
    
    public class DeviceSimulatorApiClient : ApiClient, IDeviceSimulatorApiClient
    {
        public DeviceSimulatorApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) 
            : base(httpClient, options)
        {
        }

        public Task<HttpResponseMessage> TriggerOccupancyChange(string simulatorId)
        {
            return PostEmptyContentAsync($"device-simulator/devices/{simulatorId}/trigger-occupancy-change");
        }

        public Task<HttpResponseMessage> AddSimulatedDeviceAsync(SimulatedDeviceModel model)
        {
            return PostAsJsonAsync("device-simulator/devices", model);
        }

        public Task<HttpResponseMessage> ResetDeviceSimulatorAsync()
        {
            return PostEmptyContentAsync("device-simulator/reset");
        }
    }
}