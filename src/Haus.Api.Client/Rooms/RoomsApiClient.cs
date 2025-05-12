using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Rooms;

public interface IRoomsApiClient : IApiClient
{
    Task<ListResult<RoomModel>> GetRoomsAsync();
    Task<RoomModel?> GetRoomAsync(long id);
    Task<ListResult<DeviceModel>> GetDevicesInRoomAsync(long roomId);
    Task<HttpResponseMessage> CreateRoomAsync(RoomModel model);
    Task<HttpResponseMessage> UpdateRoomAsync(long id, RoomModel model);
    Task<HttpResponseMessage> AddDevicesToRoomAsync(long roomId, params long[] deviceIds);
    Task<HttpResponseMessage> ChangeRoomLightingAsync(long roomId, LightingModel model);
    Task<HttpResponseMessage> TurnRoomOnAsync(long roomId);
    Task<HttpResponseMessage> TurnRoomOffAsync(long roomId);
}

public class RoomsApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options),
        IRoomsApiClient
{
    public Task<RoomModel?> GetRoomAsync(long id)
    {
        return GetAsJsonAsync<RoomModel>($"rooms/{id}");
    }

    public async Task<ListResult<DeviceModel>> GetDevicesInRoomAsync(long roomId)
    {
        return await GetAsJsonAsync<ListResult<DeviceModel>>($"rooms/{roomId}/devices")
            ?? new ListResult<DeviceModel>();
    }

    public Task<HttpResponseMessage> CreateRoomAsync(RoomModel model)
    {
        return PostAsJsonAsync("rooms", model);
    }

    public async Task<ListResult<RoomModel>> GetRoomsAsync()
    {
        return await GetAsJsonAsync<ListResult<RoomModel>>("rooms") ?? new ListResult<RoomModel>();
    }

    public Task<HttpResponseMessage> UpdateRoomAsync(long id, RoomModel model)
    {
        return PutAsJsonAsync($"rooms/{id}", model);
    }

    public Task<HttpResponseMessage> AddDevicesToRoomAsync(long roomId, params long[] deviceIds)
    {
        return PostAsJsonAsync($"rooms/{roomId}/add-devices", deviceIds);
    }

    public Task<HttpResponseMessage> ChangeRoomLightingAsync(long roomId, LightingModel model)
    {
        return PutAsJsonAsync($"rooms/{roomId}/lighting", model);
    }

    public Task<HttpResponseMessage> TurnRoomOnAsync(long roomId)
    {
        return PostEmptyContentAsync($"rooms/{roomId}/turn-on");
    }

    public Task<HttpResponseMessage> TurnRoomOffAsync(long roomId)
    {
        return PostEmptyContentAsync($"rooms/{roomId}/turn-off");
    }
}
