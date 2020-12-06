using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Rooms
{
    public interface IRoomsApiClient : IApiClient
    {
        Task<ListResult<RoomModel>> GetRoomsAsync();
        Task<RoomModel> GetRoomAsync(long id);
        Task<ListResult<DeviceModel>> GetDevicesInRoomAsync(long roomId);
        Task<HttpResponseMessage> CreateRoomAsync(RoomModel model);
        Task<HttpResponseMessage> UpdateRoomAsync(long id, RoomModel model);
        Task<HttpResponseMessage> AddDevicesToRoomAsync(long roomId, params long[] deviceIds);
    }
    
    public class RoomsApiClient : ApiClient, IRoomsApiClient
    {
        public RoomsApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) 
            : base(httpClient, options)
        {
        }

        public Task<RoomModel> GetRoomAsync(long id)
        {
            return GetAsJsonAsync<RoomModel>($"rooms/{id}");
        }

        public Task<ListResult<DeviceModel>> GetDevicesInRoomAsync(long roomId)
        {
            return GetAsJsonAsync<ListResult<DeviceModel>>($"rooms/{roomId}/devices");
        }

        public Task<HttpResponseMessage> CreateRoomAsync(RoomModel model)
        {
            return PostAsJsonAsync("rooms", model);
        }

        public Task<ListResult<RoomModel>> GetRoomsAsync()
        {
            return GetAsJsonAsync<ListResult<RoomModel>>("rooms");
        }

        public Task<HttpResponseMessage> UpdateRoomAsync(long id, RoomModel model)
        {
            return PutAsJsonAsync($"rooms/{id}", model);
        }

        public Task<HttpResponseMessage> AddDevicesToRoomAsync(long roomId, params long[] deviceIds)
        {
            return PostAsJsonAsync($"rooms/{roomId}/add-devices", deviceIds);
        }
    }
}