using System;
using System.Net.Http;
using Haus.Api.Client.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausApiClient(this IServiceCollection services, Action<HausApiClientSettings> configureSettings)
        {
            services.AddOptions<HausApiClientSettings>()
                .Configure(configureSettings);
            
            return services.AddHttpClient()
                .AddTransient<IHausApiClientFactory>(p =>
                {
                    var httpClientFactory = p.GetRequiredService<IHttpClientFactory>();
                    var options = p.GetRequiredService<IOptions<HausApiClientSettings>>();
                    return new HausApiClientFactory(httpClientFactory, options);
                });
        }
    }
}