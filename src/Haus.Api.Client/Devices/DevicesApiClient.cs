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
    Task<DeviceModel> GetDeviceAsync(long id);
    Task<ListResult<DeviceType>> GetDeviceTypesAsync();
    Task<ListResult<LightType>> GetLightTypesAsync();
    Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null);
    Task UpdateDeviceAsync(long deviceId, DeviceModel model);
    Task<HttpResponseMessage> ChangeDeviceLightingAsync(long deviceId, LightingModel model);
    Task<HttpResponseMessage> ChangeDeviceLightingConstraintsAsync(long deviceId, LightingConstraintsModel model);
    Task<HttpResponseMessage> TurnLightOffAsync(long deviceId);
    Task<HttpResponseMessage> TurnLightOnAsync(long deviceId);
}

public class DevicesApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options), IDeviceApiClient
{
    public Task<DeviceModel> GetDeviceAsync(long id)
    {
        return GetAsJsonAsync<DeviceModel>($"devices/{id}");
    }

    public Task<ListResult<DeviceType>> GetDeviceTypesAsync()
    {
        return GetAsJsonAsync<ListResult<DeviceType>>("device-types");
    }

    public Task<ListResult<LightType>> GetLightTypesAsync()
    {
        return GetAsJsonAsync<ListResult<LightType>>("light-types");
    }

    public Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null)
    {
        return GetAsJsonAsync<ListResult<DeviceModel>>("devices", new QueryParameters
        {
            { nameof(externalId), externalId }
        });
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