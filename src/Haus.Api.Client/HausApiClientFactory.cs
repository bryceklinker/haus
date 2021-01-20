using System.Net.Http;
using Haus.Api.Client.ClientSettings;
using Haus.Api.Client.Devices;
using Haus.Api.Client.DeviceSimulator;
using Haus.Api.Client.Diagnostics;
using Haus.Api.Client.Discovery;
using Haus.Api.Client.Logs;
using Haus.Api.Client.Options;
using Haus.Api.Client.Rooms;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client
{
    public interface IHausApiClientFactory
    {
        IHausApiClient Create();
        IDeviceApiClient CreateDeviceClient();
        IDiagnosticsApiClient CreateDiagnosticsClient();
        IRoomsApiClient CreateRoomsClient();
        IDeviceSimulatorApiClient CreateDeviceSimulatorClient();
        IDiscoveryApiClient CreateDiscoveryClient();
        ILogsApiClient CreateLogsClient();
        IClientSettingsApiClient CreateClientSettingsClient();
    }

    public class HausApiClientFactory : IHausApiClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<HausApiClientSettings> _options;

        public HausApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<HausApiClientSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public IHausApiClient Create()
        {
            return new HausApiClient(this, _httpClientFactory.CreateClient(), _options);
        }

        public IDeviceApiClient CreateDeviceClient()
        {
            return new DevicesApiClient(_httpClientFactory.CreateClient(), _options);
        }

        public IDiagnosticsApiClient CreateDiagnosticsClient()
        {
            return new DiagnosticsApiClient(_httpClientFactory.CreateClient(), _options);
        }

        public IRoomsApiClient CreateRoomsClient()
        {
            return new RoomsApiClient(_httpClientFactory.CreateClient(), _options);
        }

        public IDeviceSimulatorApiClient CreateDeviceSimulatorClient()
        {
            return new DeviceSimulatorApiClient(_httpClientFactory.CreateClient(), _options);
        }

        public IDiscoveryApiClient CreateDiscoveryClient()
        {
            return new DiscoveryApiClient(_httpClientFactory.CreateClient(), _options);
        }

        public ILogsApiClient CreateLogsClient()
        {
            return new LogsApiClient(_httpClientFactory.CreateClient(), _options);
        }

        public IClientSettingsApiClient CreateClientSettingsClient()
        {
            return new ClientSettingsApiClient(_httpClientFactory.CreateClient(), _options);
        }
    }
}