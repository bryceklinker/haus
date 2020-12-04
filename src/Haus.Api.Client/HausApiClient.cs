using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Devices;
using Haus.Api.Client.Diagnostics;
using Haus.Api.Client.Options;
using Haus.Api.Client.Rooms;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Diagnostics;
using Haus.Core.Models.Rooms;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client
{
    public interface IHausApiClient : IDeviceApiClient, IDiagnosticsApiClient, IRoomsApiClient
    {
        
    }

    public class HausApiClient : ApiClient, IHausApiClient
    {
        private readonly IHausApiClientFactory _factory;
        private IDeviceApiClient DeviceApiClient => _factory.CreateDeviceClient();
        private IDiagnosticsApiClient DiagnosticsApiClient => _factory.CreateDiagnosticsClient();
        private IRoomsApiClient RoomsApiClient => _factory.CreateRoomsClient();
        
        public HausApiClient(IHausApiClientFactory factory, HttpClient httpClient, IOptions<HausApiClientSettings> options)
            : base(httpClient, options)
        {
            _factory = factory;
        }

        public Task<DeviceModel> GetDeviceAsync(long id)
        {
            return DeviceApiClient.GetDeviceAsync(id);
        }

        public Task StartDiscovery()
        {
            return DeviceApiClient.StartDiscovery();
        }

        public Task StopDiscovery()
        {
            return DeviceApiClient.StopDiscovery();
        }

        public Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null)
        {
            return DeviceApiClient.GetDevicesAsync(externalId);
        }

        public Task UpdateDeviceAsync(long deviceId, DeviceModel model)
        {
            return DeviceApiClient.UpdateDeviceAsync(deviceId, model);
        }

        public Task<HttpResponseMessage> ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model)
        {
            return DiagnosticsApiClient.ReplayDiagnosticsMessageAsync(model);
        }

        public Task<HttpResponseMessage> CreateRoomAsync(RoomModel model)
        {
            return RoomsApiClient.CreateRoomAsync(model);
        }

        public Task<ListResult<RoomModel>> GetRoomsAsync()
        {
            return RoomsApiClient.GetRoomsAsync();
        }

        public Task<RoomModel> GetRoomAsync(long id)
        {
            return RoomsApiClient.GetRoomAsync(id);
        }
        
        public Task<HttpResponseMessage> UpdateRoomAsync(long id, RoomModel model)
        {
            return RoomsApiClient.UpdateRoomAsync(id, model);
        }
    }
}