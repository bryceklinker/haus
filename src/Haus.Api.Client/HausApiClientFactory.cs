using System.Net.Http;
using Haus.Api.Client.Application;
using Haus.Api.Client.ClientSettings;
using Haus.Api.Client.Devices;
using Haus.Api.Client.DeviceSimulator;
using Haus.Api.Client.Diagnostics;
using Haus.Api.Client.Discovery;
using Haus.Api.Client.Logs;
using Haus.Api.Client.Options;
using Haus.Api.Client.Rooms;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client;

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
    IApplicationApiClient CreateApplicationClient();
}

public class HausApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<HausApiClientSettings> options)
    : IHausApiClientFactory
{
    public IHausApiClient Create()
    {
        return new HausApiClient(this, CreateClient(), options);
    }

    public IDeviceApiClient CreateDeviceClient()
    {
        return new DevicesApiClient(CreateClient(), options);
    }

    public IDiagnosticsApiClient CreateDiagnosticsClient()
    {
        return new DiagnosticsApiClient(CreateClient(), options);
    }

    public IRoomsApiClient CreateRoomsClient()
    {
        return new RoomsApiClient(CreateClient(), options);
    }

    public IDeviceSimulatorApiClient CreateDeviceSimulatorClient()
    {
        return new DeviceSimulatorApiClient(CreateClient(), options);
    }

    public IDiscoveryApiClient CreateDiscoveryClient()
    {
        return new DiscoveryApiClient(CreateClient(), options);
    }

    public ILogsApiClient CreateLogsClient()
    {
        return new LogsApiClient(CreateClient(), options);
    }

    public IClientSettingsApiClient CreateClientSettingsClient()
    {
        return new ClientSettingsApiClient(CreateClient(), options);
    }

    public IApplicationApiClient CreateApplicationClient()
    {
        return new ApplicationApiClient(CreateClient(), options);
    }

    private HttpClient CreateClient()
    {
        return httpClientFactory.CreateClient(HausApiClientNames.Default);
    }
}
