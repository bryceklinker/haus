using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Lighting;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Lighting
{
    public interface ILightingConstraintsApiClient
    {
        Task<LightingConstraintsModel> GetDefaultLightingConstraintsAsync();
        Task<HttpResponseMessage> UpdateDefaultLightingConstraintsAsync(LightingConstraintsModel model);
    }
    
    public class LightingConstraintsApiClient : ApiClient, ILightingConstraintsApiClient
    {
        public LightingConstraintsApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) 
            : base(httpClient, options)
        {
        }

        public Task<LightingConstraintsModel> GetDefaultLightingConstraintsAsync()
        {
            return GetAsJsonAsync<LightingConstraintsModel>("lighting-constraints/defaults");
        }

        public Task<HttpResponseMessage> UpdateDefaultLightingConstraintsAsync(LightingConstraintsModel model)
        {
            return PutAsJsonAsync("lighting-constraints/defaults", model);
        }
    }
}