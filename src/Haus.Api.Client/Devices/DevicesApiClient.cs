using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Devices;

public interface IDeviceApiClient : IApiClient
{
    Task<DeviceModel?> GetDeviceAsync(long id);
    Task<ListResult<DeviceType>> GetDeviceTypesAsync();
    Task<ListResult<LightType>> GetLightTypesAsync();
    Task<ListResult<DeviceModel>> GetDevicesAsync(string? externalId = null);
    Task UpdateDeviceAsync(long deviceId, DeviceModel model);
    Task<HttpResponseMessage> ChangeDeviceLightingAsync(long deviceId, LightingModel model);
    Task<HttpResponseMessage> ChangeDeviceLightingConstraintsAsync(long deviceId, LightingConstraintsModel model);
    Task<HttpResponseMessage> TurnLightOffAsync(long deviceId);
    Task<HttpResponseMessage> TurnLightOnAsync(long deviceId);
}

public class DevicesApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options), IDeviceApiClient
{
    public Task<DeviceModel?> GetDeviceAsync(long id)
    {
        return GetAsJsonAsync<DeviceModel>($"devices/{id}");
    }

    public async Task<ListResult<DeviceType>> GetDeviceTypesAsync()
    {
        var result = await GetAsJsonAsync<ListResult<DeviceType>>("device-types").ConfigureAwait(false);
        return result ?? new ListResult<DeviceType>();
    }

    public async Task<ListResult<LightType>> GetLightTypesAsync()
    {
        var result = await GetAsJsonAsync<ListResult<LightType>>("light-types").ConfigureAwait(false);
        return result ?? new ListResult<LightType>();
    }

    public async Task<ListResult<DeviceModel>> GetDevicesAsync(string? externalId = null)
    {
        var queryParameters = new QueryParameters();
        if (!string.IsNullOrWhiteSpace(externalId)) queryParameters.Add("externalId", externalId);
        var result = await GetAsJsonAsync<ListResult<DeviceModel>>("devices", queryParameters).ConfigureAwait(false);
        return result ?? new ListResult<DeviceModel>();
    }

    public Task UpdateDeviceAsync(long deviceId, DeviceModel model)
    {
        return PutAsJsonAsync($"devices/{deviceId}", model);
    }

    public Task<HttpResponseMessage> ChangeDeviceLightingAsync(long deviceId, LightingModel model)
    {
        return PutAsJsonAsync($"devices/{deviceId}/lighting", model);
    }

    public Task<HttpResponseMessage> ChangeDeviceLightingConstraintsAsync(long deviceId, LightingConstraintsModel model)
    {
        return PutAsJsonAsync($"devices/{deviceId}/lighting-constraints", model);
    }

    public Task<HttpResponseMessage> TurnLightOffAsync(long deviceId)
    {
        return PostEmptyContentAsync($"devices/{deviceId}/turn-off");
    }

    public Task<HttpResponseMessage> TurnLightOnAsync(long deviceId)
    {
        return PostEmptyContentAsync($"devices/{deviceId}/turn-on");
    }
}