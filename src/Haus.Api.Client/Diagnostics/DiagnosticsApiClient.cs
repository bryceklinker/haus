using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Diagnostics;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Diagnostics;

public interface IDiagnosticsApiClient : IApiClient
{
    Task<HttpResponseMessage> ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model);
}

public class DiagnosticsApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options), IDiagnosticsApiClient
{
    public Task<HttpResponseMessage> ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model)
    {
        return PostAsJsonAsync("diagnostics/replay", model);
    }
}