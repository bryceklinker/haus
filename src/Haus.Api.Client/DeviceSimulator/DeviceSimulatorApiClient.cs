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
        Task<HttpResponseMessage> AddSimulatedDevice(CreateSimulatedDeviceModel model);
    }
    
    public class DeviceSimulatorApiClient : ApiClient, IDeviceSimulatorApiClient
    {
        public DeviceSimulatorApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) 
            : base(httpClient, options)
        {
        }

        public Task<HttpResponseMessage> AddSimulatedDevice(CreateSimulatedDeviceModel model)
        {
            return PostAsJsonAsync("device-simulator/devices", model);
        }
    }
}