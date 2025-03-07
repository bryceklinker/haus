using System;
using Haus.Api.Client;
using Haus.Site.Host.Shared.Realtime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Haus.Site.Host;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausSiteServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiUrl = configuration.GetValue<string>("Api:BaseUrl");
        var authDomain = configuration.GetValue<string>("Auth:Domain");
        var authClientId = configuration.GetValue<string>("Auth:ClientId");
        var authAudience = configuration.GetValue<string>("Auth:Audience");

        ArgumentException.ThrowIfNullOrEmpty(apiUrl, nameof(apiUrl));
        ArgumentException.ThrowIfNullOrEmpty(authDomain, nameof(authDomain));
        ArgumentException.ThrowIfNullOrEmpty(authClientId, nameof(authClientId));
        ArgumentException.ThrowIfNullOrEmpty(authAudience, nameof(authAudience));

        services.AddMudServices();
        services.AddScoped<AuthorizationMessageHandler>(sp =>
        {
            var tokenProvider = sp.GetRequiredService<IAccessTokenProvider>();
            var navigation = sp.GetRequiredService<NavigationManager>();
            var handler = new AuthorizationMessageHandler(tokenProvider, navigation);
            handler.ConfigureHandler([apiUrl]);
            return handler;
        });
        services
            .AddHausApiClient(opts =>
            {
                opts.BaseUrl = apiUrl;
            })
            .AddHttpMessageHandler<AuthorizationMessageHandler>();

        services
            .AddCascadingAuthenticationState()
            .AddAuthorizationCore()
            .AddOidcAuthentication(opts =>
            {
                opts.ProviderOptions.ClientId = authClientId;
                opts.ProviderOptions.Authority = $"https://{authDomain}";
                opts.ProviderOptions.ResponseType = "code";
                opts.ProviderOptions.AdditionalProviderParameters.Add("audience", authAudience);
            });

        services.AddScoped<IRealtimeDataFactory, SignalRRealtimeDataFactory>();
        return services;
    }
}
