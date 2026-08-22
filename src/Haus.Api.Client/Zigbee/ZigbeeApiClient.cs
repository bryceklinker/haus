using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Zigbee;

public interface IZigbeeApiClient : IApiClient
{
    Task<ZigbeeConnectionStatusModel?> GetZigbeeStatusAsync();
    Task<ListResult<ZigbeeActivityEntryModel>> GetZigbeeActivityAsync();
    Task<ListResult<ZigbeeKnownDeviceModel>> GetZigbeeDevicesAsync();
}

public class ZigbeeApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options),
        IZigbeeApiClient
{
    public Task<ZigbeeConnectionStatusModel?> GetZigbeeStatusAsync()
    {
        return GetAsJsonAsync<ZigbeeConnectionStatusModel>("zigbee/status");
    }

    public async Task<ListResult<ZigbeeActivityEntryModel>> GetZigbeeActivityAsync()
    {
        return await GetAsJsonAsync<ListResult<ZigbeeActivityEntryModel>>("zigbee/activity")
            ?? new ListResult<ZigbeeActivityEntryModel>();
    }

    public async Task<ListResult<ZigbeeKnownDeviceModel>> GetZigbeeDevicesAsync()
    {
        return await GetAsJsonAsync<ListResult<ZigbeeKnownDeviceModel>>("zigbee/devices")
            ?? new ListResult<ZigbeeKnownDeviceModel>();
    }
}
