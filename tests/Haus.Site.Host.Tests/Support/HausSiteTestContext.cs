using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Site.Host.Shared.Realtime;
using Haus.Site.Host.Tests.Support.Http;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

namespace Haus.Site.Host.Tests.Support;

public class HausSiteTestContext : IAsyncLifetime
{
    private readonly InMemoryHttpClientFactory _httpClientFactory;
    private readonly InMemoryRealtimeDataFactory _realtimeDataFactory;
    protected TestContext Context { get; }
    protected InMemoryHttpMessageHandler HausApiHandler => _httpClientFactory.GetHandler(HausApiClientNames.Default);

    protected IRenderedComponent<T> RenderView<T>(
        Action<ComponentParameterCollectionBuilder<T>>? parameterBuilder = null
    )
        where T : IComponent
    {
        return Context.RenderComponent(parameterBuilder);
    }

    protected HausSiteTestContext()
    {
        Context = new TestContext();
        Context.JSInterop.Mode = JSRuntimeMode.Loose;

        Context.Services.AddMudServices(opts =>
        {
            opts.SnackbarConfiguration.ShowTransitionDuration = 0;
            opts.SnackbarConfiguration.HideTransitionDuration = 0;
            opts.PopoverOptions.CheckForPopoverProvider = false;
        });
        Context.Services.AddHausApiClient(opts =>
        {
            opts.BaseUrl = ConfigureHttpResponseOptions.DefaultBaseUrl;
        });

        _httpClientFactory = new InMemoryHttpClientFactory();
        Context.Services.AddSingleton(_httpClientFactory);
        Context.Services.Replace<IHttpClientFactory>(_httpClientFactory);

        _realtimeDataFactory = new InMemoryRealtimeDataFactory();
        Context.Services.AddSingleton(_realtimeDataFactory);
        Context.Services.Replace<IRealtimeDataFactory>(_realtimeDataFactory);
    }

    public async Task InitializeAsync()
    {
        var hostedServices = Context.Services.GetServices<IHostedService>();
        foreach (var service in hostedServices)
        {
            await service.StartAsync(CancellationToken.None);
        }
    }

    public async Task DisposeAsync()
    {
        var hostedServices = Context.Services.GetServices<IHostedService>();
        foreach (var service in hostedServices)
        {
            await service.StartAsync(CancellationToken.None);
        }

        await Context.Services.DisposeAsync();
    }

    protected InMemoryRealtimeDataSubscriber GetSubscriber(string hubName)
    {
        return _realtimeDataFactory.GetSubscriber(hubName);
    }
}
