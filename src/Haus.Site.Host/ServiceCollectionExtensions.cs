using System;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Haus.Api.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Haus.Site.Host;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausSiteServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiUrl = configuration.GetValue<string>("Api:BaseUrl");
        ArgumentException.ThrowIfNullOrEmpty(apiUrl, nameof(apiUrl));
        
        services.AddMudServices();
        services.AddFluxor(opts =>
        {
            opts.ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly)
                .UseReduxDevTools();
        });
        services.AddHausApiClient(opts =>
        {
            opts.BaseUrl = apiUrl;
        });
        
        return services;
    }
}