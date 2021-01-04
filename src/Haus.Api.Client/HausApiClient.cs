using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Devices;
using Haus.Api.Client.DeviceSimulator;
using Haus.Api.Client.Diagnostics;
using Haus.Api.Client.Lighting;
using Haus.Api.Client.Options;
using Haus.Api.Client.Rooms;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Diagnostics;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client
{
    public interface IHausApiClient : 
        IDeviceApiClient, 
        IDiagnosticsApiClient, 
        IRoomsApiClient,
        IDeviceSimulatorApiClient,
        ILightingConstraintsApiClient
    {
        
    }

    public class HausApiClient : ApiClient, IHausApiClient
    {
        private readonly IHausApiClientFactory _factory;
        private IDeviceApiClient DeviceApiClient => _factory.CreateDeviceClient();
        private IDiagnosticsApiClient DiagnosticsApiClient => _factory.CreateDiagnosticsClient();
        private IRoomsApiClient RoomsApiClient => _factory.CreateRoomsClient();
        private IDeviceSimulatorApiClient DeviceSimulatorApiClient => _factory.CreateDeviceSimulatorClient();
        private ILightingConstraintsApiClient LightingConstraintsApiClient => _factory.CreateLightingApiClient();
        
        public HausApiClient(IHausApiClientFactory factory, HttpClient httpClient, IOptions<HausApiClientSettings> options)
            : base(httpClient, options)
        {
            _factory = factory;
        }

        public Task<DeviceModel> GetDeviceAsync(long id)
        {
            return DeviceApiClient.GetDeviceAsync(id);
        }

        public Task<ListResult<DeviceType>> GetDeviceTypesAsync()
        {
            return DeviceApiClient.GetDeviceTypesAsync();
        }

        public Task StartDiscoveryAsync()
        {
            return DeviceApiClient.StartDiscoveryAsync();
        }

        public Task StopDiscoveryAsync()
        {
            return DeviceApiClient.StopDiscoveryAsync();
        }

        public Task<HttpResponseMessage> SyncDevicesAsync()
        {
            return DeviceApiClient.SyncDevicesAsync();
        }

        public Task<HttpResponseMessage> ChangeDeviceLightingAsync(long deviceId, LightingModel model)
        {
            return DeviceApiClient.ChangeDeviceLightingAsync(deviceId, model);
        }

        public Task<HttpResponseMessage> TurnLightOffAsync(long deviceId)
        {
            return DeviceApiClient.TurnLightOffAsync(deviceId);
        }

        public Task<HttpResponseMessage> TurnLightOnAsync(long deviceId)
        {
            return DeviceApiClient.TurnLightOnAsync(deviceId);
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

        public Task<ListResult<DeviceModel>> GetDevicesInRoomAsync(long roomId)
        {
            return RoomsApiClient.GetDevicesInRoomAsync(roomId);
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

        public Task<HttpResponseMessage> AddDevicesToRoomAsync(long roomId, params long[] deviceIds)
        {
            return RoomsApiClient.AddDevicesToRoomAsync(roomId, deviceIds);
        }

        public Task<HttpResponseMessage> ChangeRoomLightingAsync(long roomId, LightingModel model)
        {
            return RoomsApiClient.ChangeRoomLightingAsync(roomId, model);
        }

        public Task<HttpResponseMessage> TurnRoomOnAsync(long roomId)
        {
            return RoomsApiClient.TurnRoomOnAsync(roomId);
        }

        public Task<HttpResponseMessage> TurnRoomOffAsync(long roomId)
        {
            return RoomsApiClient.TurnRoomOffAsync(roomId);
        }

        public Task<HttpResponseMessage> AddSimulatedDeviceAsync(SimulatedDeviceModel model)
        {
            return DeviceSimulatorApiClient.AddSimulatedDeviceAsync(model);
        }

        public Task<HttpResponseMessage> ResetDeviceSimulatorAsync()
        {
            return DeviceSimulatorApiClient.ResetDeviceSimulatorAsync();
        }

        public Task<LightingConstraintsModel> GetDefaultLightingConstraintsAsync()
        {
            return LightingConstraintsApiClient.GetDefaultLightingConstraintsAsync();
        }

        public Task<HttpResponseMessage> UpdateDefaultLightingConstraintsAsync(LightingConstraintsModel model)
        {
            return LightingConstraintsApiClient.UpdateDefaultLightingConstraintsAsync(model);
        }
    }
}