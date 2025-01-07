using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Application;
using Haus.Api.Client.ClientSettings;
using Haus.Api.Client.Common;
using Haus.Api.Client.Devices;
using Haus.Api.Client.DeviceSimulator;
using Haus.Api.Client.Diagnostics;
using Haus.Api.Client.Discovery;
using Haus.Api.Client.Logs;
using Haus.Api.Client.Options;
using Haus.Api.Client.Rooms;
using Haus.Core.Models.Application;
using Haus.Core.Models.ClientSettings;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Diagnostics;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Logs;
using Haus.Core.Models.Rooms;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client;

public interface IHausApiClient :
    IDeviceApiClient,
    IDiagnosticsApiClient,
    IRoomsApiClient,
    IDeviceSimulatorApiClient,
    IDiscoveryApiClient,
    ILogsApiClient,
    IClientSettingsApiClient,
    IApplicationApiClient
{
}

public class HausApiClient(
    IHausApiClientFactory factory,
    HttpClient httpClient,
    IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options), IHausApiClient
{
    private IDeviceApiClient DeviceApiClient => factory.CreateDeviceClient();
    private IDiagnosticsApiClient DiagnosticsApiClient => factory.CreateDiagnosticsClient();
    private IRoomsApiClient RoomsApiClient => factory.CreateRoomsClient();
    private IDeviceSimulatorApiClient DeviceSimulatorApiClient => factory.CreateDeviceSimulatorClient();
    private IDiscoveryApiClient DiscoveryApiClient => factory.CreateDiscoveryClient();
    private ILogsApiClient LogsApiClient => factory.CreateLogsClient();
    private IClientSettingsApiClient ClientSettingsApiClient => factory.CreateClientSettingsClient();
    private IApplicationApiClient ApplicationApiClient => factory.CreateApplicationClient();

    public Task<DiscoveryModel> GetDiscoveryStateAsync()
    {
        return DiscoveryApiClient.GetDiscoveryStateAsync();
    }

    public Task<HttpResponseMessage> StartDiscoveryAsync()
    {
        return DiscoveryApiClient.StartDiscoveryAsync();
    }

    public Task<HttpResponseMessage> StopDiscoveryAsync()
    {
        return DiscoveryApiClient.StopDiscoveryAsync();
    }

    public Task<HttpResponseMessage> SyncDevicesAsync()
    {
        return DiscoveryApiClient.SyncDevicesAsync();
    }

    public Task<DeviceModel> GetDeviceAsync(long id)
    {
        return DeviceApiClient.GetDeviceAsync(id);
    }

    public Task<ListResult<DeviceType>> GetDeviceTypesAsync()
    {
        return DeviceApiClient.GetDeviceTypesAsync();
    }

    public Task<ListResult<LightType>> GetLightTypesAsync()
    {
        return DeviceApiClient.GetLightTypesAsync();
    }

    public Task<HttpResponseMessage> ChangeDeviceLightingAsync(long deviceId, LightingModel model)
    {
        return DeviceApiClient.ChangeDeviceLightingAsync(deviceId, model);
    }

    public Task<HttpResponseMessage> ChangeDeviceLightingConstraintsAsync(long deviceId, LightingConstraintsModel model)
    {
        return DeviceApiClient.ChangeDeviceLightingConstraintsAsync(deviceId, model);
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

    public Task<HttpResponseMessage> TriggerOccupancyChange(string simulatorId)
    {
        return DeviceSimulatorApiClient.TriggerOccupancyChange(simulatorId);
    }

    public Task<HttpResponseMessage> AddSimulatedDeviceAsync(SimulatedDeviceModel model)
    {
        return DeviceSimulatorApiClient.AddSimulatedDeviceAsync(model);
    }

    public Task<HttpResponseMessage> ResetDeviceSimulatorAsync()
    {
        return DeviceSimulatorApiClient.ResetDeviceSimulatorAsync();
    }

    public Task<ListResult<LogEntryModel>> GetLogsAsync(GetLogsParameters parameters = null)
    {
        return LogsApiClient.GetLogsAsync(parameters);
    }

    public Task<ClientSettingsModel> GetClientSettingsAsync()
    {
        return ClientSettingsApiClient.GetClientSettingsAsync();
    }

    public Task<ApplicationVersionModel> GetLatestVersionAsync()
    {
        return ApplicationApiClient.GetLatestVersionAsync();
    }

    public Task<ListResult<ApplicationPackageModel>> GetLatestPackagesAsync()
    {
        return ApplicationApiClient.GetLatestPackagesAsync();
    }

    public Task<HttpResponseMessage> DownloadLatestPackageAsync(int packageId)
    {
        return ApplicationApiClient.DownloadLatestPackageAsync(packageId);
    }
}