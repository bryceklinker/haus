using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Devices
{
    public interface IDeviceApiClient
    {
        Task<DeviceModel> GetDeviceAsync(long id);
        Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null);
        Task UpdateDeviceAsync(long deviceId, DeviceModel model);
        Task StartDiscovery();
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

        public async Task StartDiscovery()
        {
            var response = await PostEmptyContentAsync("devices/start-discovery")
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}