using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Zigbee.Models.Diagnostics;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Zigbee;

public interface IZigbeeHostApiClient : IApiClient
{
    Task<ZigbeeConnectionStatusModel?> GetStatusAsync();
    Task<IReadOnlyList<ZigbeeActivityEntryModel>> GetActivityAsync();
    Task<IReadOnlyList<ZigbeeKnownDeviceModel>> GetDevicesAsync();
}

public class ZigbeeHostApiClient(HttpClient httpClient, IOptions<ZigbeeHostApiClientSettings> options)
    : ApiClient<ZigbeeHostApiClientSettings>(httpClient, options),
        IZigbeeHostApiClient
{
    public Task<ZigbeeConnectionStatusModel?> GetStatusAsync()
    {
        return GetAsJsonAsync<ZigbeeConnectionStatusModel>("zigbee/status");
    }

    public async Task<IReadOnlyList<ZigbeeActivityEntryModel>> GetActivityAsync()
    {
        return await GetAsJsonAsync<List<ZigbeeActivityEntryModel>>("zigbee/activity") ?? [];
    }

    public async Task<IReadOnlyList<ZigbeeKnownDeviceModel>> GetDevicesAsync()
    {
        return await GetAsJsonAsync<List<ZigbeeKnownDeviceModel>>("zigbee/devices") ?? [];
    }
}
