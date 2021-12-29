using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.ClientSettings;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.ClientSettings;

public interface IClientSettingsApiClient
{
    Task<ClientSettingsModel> GetClientSettingsAsync();
}

public class ClientSettingsApiClient : ApiClient, IClientSettingsApiClient
{
    public ClientSettingsApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) 
        : base(httpClient, options)
    {
    }

    public Task<ClientSettingsModel> GetClientSettingsAsync()
    {
        var url = UrlUtility.Join(null, BaseUrl, "client-settings");
        return HttpClient.GetFromJsonAsync<ClientSettingsModel>(url);
    }
}