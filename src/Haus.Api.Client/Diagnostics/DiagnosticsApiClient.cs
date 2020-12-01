using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Diagnostics;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Diagnostics
{
    public interface IDiagnosticsApiClient
    {
        Task ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model);
    }
    
    public class DiagnosticsApiClient : ApiClient, IDiagnosticsApiClient
    {
        public DiagnosticsApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) 
            : base(httpClient, options)
        {
        }

        public Task ReplayDiagnosticsMessageAsync(MqttDiagnosticsMessageModel model)
        {
            return PostAsJsonAsync("diagnostics/replay", model);
        }
    }
}