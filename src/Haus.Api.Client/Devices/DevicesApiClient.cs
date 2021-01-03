using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Devices
{
    public interface IDeviceApiClient : IApiClient
    {
        Task<DeviceModel> GetDeviceAsync(long id);
        Task<ListResult<DeviceType>> GetDeviceTypesAsync();
        Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null);
        Task UpdateDeviceAsync(long deviceId, DeviceModel model);
        Task StartDiscoveryAsync();
        Task StopDiscoveryAsync();
        Task<HttpResponseMessage> SyncDevicesAsync();
        Task<HttpResponseMessage> ChangeDeviceLightingAsync(long deviceId, LightingModel model);
        Task<HttpResponseMessage> TurnLightOffAsync(long deviceId);
        Task<HttpResponseMessage> TurnLightOnAsync(long deviceId);
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

        public Task<ListResult<DeviceType>> GetDeviceTypesAsync()
        {
            return GetAsJsonAsync<ListResult<DeviceType>>("device-types");
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

        public Task<HttpResponseMessage> SyncDevicesAsync()
        {
            return PostEmptyContentAsync("devices/sync-discovery");
        }

        public Task<HttpResponseMessage> ChangeDeviceLightingAsync(long deviceId, LightingModel model)
        {
            return PostAsJsonAsync($"devices/{deviceId}/lighting", model);
        }

        public Task<HttpResponseMessage> TurnLightOffAsync(long deviceId)
        {
            return PostEmptyContentAsync($"devices/{deviceId}/turn-off");
        }
        
        public Task<HttpResponseMessage> TurnLightOnAsync(long deviceId)
        {
            return PostEmptyContentAsync($"devices/{deviceId}/turn-on");
        }

        public async Task StartDiscoveryAsync()
        {
            var response = await PostEmptyContentAsync("devices/start-discovery")
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task StopDiscoveryAsync()
        {
            var response = await PostEmptyContentAsync("devices/stop-discovery")
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}