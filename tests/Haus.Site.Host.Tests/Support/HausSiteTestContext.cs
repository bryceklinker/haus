using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions.Common;
using Haus.Api.Client;
using Haus.Site.Host.Shared.Realtime;
using Haus.Site.Host.Tests.Support.Http;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor;
using MudBlazor.Services;

namespace Haus.Site.Host.Tests.Support;

public class HausSiteTestContext : IAsyncLifetime
{
    private readonly InMemoryHttpClientFactory _httpClientFactory;
    private readonly InMemoryRealtimeDataFactory _realtimeDataFactory;
    protected BunitContext Context { get; }
    protected BunitAuthorizationContext AuthContext { get; }
    protected InMemoryHttpMessageHandler HausApiHandler => _httpClientFactory.GetHandler(HausApiClientNames.Default);
    protected BunitNavigationManager NavigationManager => Context.Services.GetRequiredService<BunitNavigationManager>();

    protected HausSiteTestContext()
    {
        Context = new BunitContext();
        Context.JSInterop.Mode = JSRuntimeMode.Loose;

        Context.Services.AddMudServices(opts =>
        {
            opts.SnackbarConfiguration.ShowTransitionDuration = 0;
            opts.SnackbarConfiguration.HideTransitionDuration = 0;
            opts.PopoverOptions.CheckForPopoverProvider = false;
        });

        Context.Services.AddCascadingAuthenticationState();
        Context.Services.AddAuthorizationCore();

        AuthContext = Context.AddAuthorization();
        AuthContext.SetAuthorized("bob");

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

    public virtual async Task InitializeAsync()
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

    protected InMemoryRealtimeDataSubscriber GetSubscriber(string source)
    {
        return _realtimeDataFactory.GetSubscriber(source);
    }

    protected IRenderedComponent<T> RenderView<T>(
        Action<ComponentParameterCollectionBuilder<T>>? parameterBuilder = null
    )
        where T : IComponent
    {
        return Context.Render(parameterBuilder);
    }

    protected async Task<IRenderedComponent<MudDialogProvider>> RenderDialogAsync<T>()
        where T : IComponent
    {
        var provider = Context.Render<MudDialogProvider>();
        var dialogService = Context.Services.GetRequiredService<IDialogService>();
        await provider.InvokeAsync(async () =>
        {
            await dialogService.ShowAsync<T>();
        });
        return provider;
    }
}
