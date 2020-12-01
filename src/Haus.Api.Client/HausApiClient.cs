using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Diagnostics;

namespace Haus.Api.Client
{
    public interface IHausApiClient
    {
        Task<DeviceModel> GetDeviceAsync(long id);
        Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null);
        Task UpdateDeviceAsync(long deviceId, DeviceModel model);
        Task StartDiscovery();
        Task ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model);
    }

    public class HausApiClient : IHausApiClient
    {
        private readonly HttpClient _httpClient;

        public HausApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DeviceModel> GetDeviceAsync(long id)
        {
            var url = $"/api/devices/{id}";
            return await _httpClient.GetFromJsonAsync<DeviceModel>(url)
                .ConfigureAwait(false);
        }

        public async Task StartDiscovery()
        {
            var url = $"/api/devices/start-discovery";
            var response = await _httpClient.PostAsync(url, new StringContent(""))
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null)
        {
            var url = "/api/devices";
            if (!string.IsNullOrWhiteSpace(externalId))
                url += $"?{nameof(externalId)}={HttpUtility.UrlEncode(externalId)}";

            return await _httpClient.GetFromJsonAsync<ListResult<DeviceModel>>(url)
                .ConfigureAwait(false);
        }

        public async Task UpdateDeviceAsync(long deviceId, DeviceModel model)
        {
            var url = $"/api/devices/{deviceId}";
            await _httpClient.PutAsJsonAsync(url, model)
                .ConfigureAwait(false);
        }

        public async Task ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model)
        {
            const string url = "/api/diagnostics/replay";
            await _httpClient.PostAsJsonAsync(url, model)
                .ConfigureAwait(false);
        }
    }
}