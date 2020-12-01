using System.Threading.Tasks;
using Haus.Api.Client.Devices;
using Haus.Api.Client.Diagnostics;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Diagnostics;

namespace Haus.Api.Client
{
    public interface IHausApiClient : IDeviceApiClient
    {
        Task ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model);
    }

    public class HausApiClient : IHausApiClient
    {
        private readonly IHausApiClientFactory _factory;
        private IDeviceApiClient DeviceApiClient => _factory.CreateDeviceClient();
        private IDiagnosticsApiClient DiagnosticsApiClient => _factory.CreateDiagnosticsClient();

        public HausApiClient(IHausApiClientFactory factory)
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

        public Task<ListResult<DeviceModel>> GetDevicesAsync(string externalId = null)
        {
            return DeviceApiClient.GetDevicesAsync(externalId);
        }

        public Task UpdateDeviceAsync(long deviceId, DeviceModel model)
        {
            return DeviceApiClient.UpdateDeviceAsync(deviceId, model);
        }

        public Task ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model)
        {
            return DiagnosticsApiClient.ReplayDiagnosticsMessageAsync(model);
        }
    }
}