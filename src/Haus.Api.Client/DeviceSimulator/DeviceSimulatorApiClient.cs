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
        Task<HttpResponseMessage> AddSimulatedDeviceAsync(CreateSimulatedDeviceModel model);
        Task<HttpResponseMessage> ResetDeviceSimulatorAsync();
    }
    
    public class DeviceSimulatorApiClient : ApiClient, IDeviceSimulatorApiClient
    {
        public DeviceSimulatorApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) 
            : base(httpClient, options)
        {
        }

        public Task<HttpResponseMessage> AddSimulatedDeviceAsync(CreateSimulatedDeviceModel model)
        {
            return PostAsJsonAsync("device-simulator/devices", model);
        }

        public Task<HttpResponseMessage> ResetDeviceSimulatorAsync()
        {
            return PostEmptyContentAsync("device-simulator/reset");
        }
    }
}