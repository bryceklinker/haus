using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Rooms
{
    public interface IRoomsApiClient : IApiClient
    {
        Task<ListResult<RoomModel>> GetRoomsAsync();
        Task<RoomModel> GetRoomAsync(long id);
        Task<HttpResponseMessage> CreateRoomAsync(RoomModel model);
        Task<HttpResponseMessage> UpdateRoomAsync(long id, RoomModel model);
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
    }
}