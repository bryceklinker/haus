using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Devices
{
    public interface IDeviceApiClient : IApiClient
    {
        Task<DeviceModel> GetDeviceAsync(long id);
        Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null);
        Task UpdateDeviceAsync(long deviceId, DeviceModel model);
        Task StartDiscovery();
        Task StopDiscovery();
        Task<HttpResponseMessage> ChangeDeviceLighting(long deviceId, LightingModel model);
        Task<HttpResponseMessage> TurnLightOff(long deviceId);
        Task<HttpResponseMessage> TurnLightOn(long deviceId);
    }
    
    public class DevicesApiClient : ApiClient, IDeviceApiClient 
    {
        public DevicesApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) 
            : base(httpClient, options)
        {
        }

        public Task<DeviceModel> GetDeviceAsync(long id)
        {
            return GetAsJsonAsync<DeviceModel>($"devices/{id}");
        }

        public Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null)
        {
            return GetAsJsonAsync<ListResult<DeviceModel>>("devices", new QueryParameters
            {
                {nameof(externalId), externalId}
            });
        }

        public Task UpdateDeviceAsync(long deviceId, DeviceModel model)
        {
            return PutAsJsonAsync($"devices/{deviceId}", model);
        }

        public Task<HttpResponseMessage> ChangeDeviceLighting(long deviceId, LightingModel model)
        {
            return PostAsJsonAsync($"devices/{deviceId}/lighting", model);
        }

        public Task<HttpResponseMessage> TurnLightOff(long deviceId)
        {
            return PostEmptyContentAsync($"devices/{deviceId}/turn-off");
        }
        
        public Task<HttpResponseMessage> TurnLightOn(long deviceId)
        {
            return PostEmptyContentAsync($"devices/{deviceId}/turn-on");
        }

        public async Task StartDiscovery()
        {
            var response = await PostEmptyContentAsync("devices/start-discovery")
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task StopDiscovery()
        {
            var response = await PostEmptyContentAsync("devices/stop-discovery")
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}