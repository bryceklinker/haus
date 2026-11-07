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

// Zigbee.Host is reachable only over the internal network and can be mid-restart or simply not
// running yet (e.g. local dev via `make watch` without `make zigbee-host`). The old in-memory
// mirror it replaces never threw, so connectivity failures here degrade to the same
// Unknown/empty defaults rather than surfacing as a 500 from ZigbeeController.
public class ZigbeeHostApiClient(HttpClient httpClient, IOptions<ZigbeeHostApiClientSettings> options)
    : ApiClient<ZigbeeHostApiClientSettings>(httpClient, options),
        IZigbeeHostApiClient
{
    public async Task<ZigbeeConnectionStatusModel?> GetStatusAsync()
    {
        try
        {
            return await GetAsJsonAsync<ZigbeeConnectionStatusModel>("zigbee/status");
        }
        catch (HttpRequestException)
        {
            return ZigbeeConnectionStatusModel.Unknown;
        }
    }

    public async Task<IReadOnlyList<ZigbeeActivityEntryModel>> GetActivityAsync()
    {
        try
        {
            return await GetAsJsonAsync<List<ZigbeeActivityEntryModel>>("zigbee/activity") ?? [];
        }
        catch (HttpRequestException)
        {
            return [];
        }
    }

    public async Task<IReadOnlyList<ZigbeeKnownDeviceModel>> GetDevicesAsync()
    {
        try
        {
            return await GetAsJsonAsync<List<ZigbeeKnownDeviceModel>>("zigbee/devices") ?? [];
        }
        catch (HttpRequestException)
        {
            return [];
        }
    }
}
