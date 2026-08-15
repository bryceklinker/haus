using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Haus.Api.Client.Options;
using Haus.Site.Host.Shared.Realtime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Haus.Site.Host.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void WhenBrowserHostDiffersFromConfiguredApiHostThenRegisteredApiClientUsesBrowserHost()
    {
        var services = new ServiceCollection();

        services.AddHausSiteServices(BuildConfiguration(), "https://192.168.1.50:5003/");

        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<HausApiClientSettings>>().Value;
        Assert.Equal("https://192.168.1.50:5001", settings.BaseUrl);
    }

    [Fact]
    public void WhenBrowserHostDiffersFromConfiguredApiHostThenRegisteredRealtimeFactoryUsesBrowserHost()
    {
        var services = new ServiceCollection();

        services.AddHausSiteServices(BuildConfiguration(), "https://192.168.1.50:5003/");
        services.AddSingleton<IAccessTokenProvider, StubAccessTokenProvider>();

        var factory = (SignalRRealtimeDataFactory)
            services.BuildServiceProvider().GetRequiredService<IRealtimeDataFactory>();
        Assert.Equal("https://192.168.1.50:5001", factory.ApiUrl);
    }

    [Fact]
    public async Task WhenRequestTargetsResolvedApiHostThenAuthorizationHeaderIsAttached()
    {
        var services = new ServiceCollection();
        services.AddHausSiteServices(BuildConfiguration(), "https://192.168.1.50:5003/");
        services.AddSingleton<NavigationManager>(new StubNavigationManager("https://192.168.1.50:5003/"));
        services.AddSingleton<IAccessTokenProvider, StubAccessTokenProvider>();
        var provider = services.BuildServiceProvider();
        var handler = provider.GetRequiredService<AuthorizationMessageHandler>();
        var capture = new CapturingHandler();
        handler.InnerHandler = capture;
        using var client = new HttpClient(handler);

        await client.GetAsync("https://192.168.1.50:5001/api/devices");

        Assert.NotNull(capture.LastRequest?.Headers.Authorization);
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Api:BaseUrl"] = "https://localhost:5001",
                    ["Auth:Domain"] = "haus-app.us.auth0.com",
                    ["Auth:ClientId"] = "client-id",
                    ["Auth:Audience"] = "https://haus-portal-api.com",
                }
            )
            .Build();
    }

    private sealed class StubNavigationManager : NavigationManager
    {
        public StubNavigationManager(string baseUri) => Initialize(baseUri, baseUri);
    }

    private sealed class StubAccessTokenProvider : IAccessTokenProvider
    {
        public ValueTask<AccessTokenResult> RequestAccessToken() => RequestAccessToken(new AccessTokenRequestOptions());

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            var token = new AccessToken
            {
                Value = "test-token",
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                GrantedScopes = [],
            };
            return ValueTask.FromResult(new AccessTokenResult(AccessTokenResultStatus.Success, token, null, null));
        }
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
