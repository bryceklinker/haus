using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Discovery;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Discovery;

public interface IDiscoveryApiClient
{
    Task<DiscoveryModel?> GetDiscoveryStateAsync();
    Task<HttpResponseMessage> StartDiscoveryAsync();
    Task<HttpResponseMessage> StopDiscoveryAsync();
    Task<HttpResponseMessage> SyncDevicesAsync();
}

public class DiscoveryApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options),
        IDiscoveryApiClient
{
    public Task<DiscoveryModel?> GetDiscoveryStateAsync()
    {
        return GetAsJsonAsync<DiscoveryModel>("discovery/state");
    }

    public Task<HttpResponseMessage> StartDiscoveryAsync()
    {
        return PostEmptyContentAsync("discovery/start");
    }

    public Task<HttpResponseMessage> StopDiscoveryAsync()
    {
        return PostEmptyContentAsync("discovery/stop");
    }

    public Task<HttpResponseMessage> SyncDevicesAsync()
    {
        return PostEmptyContentAsync("discovery/sync");
    }
}
