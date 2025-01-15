using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fluxor;
using Haus.Api.Client;
using Haus.Site.Host.Shared.State;
using Haus.Site.Host.Tests.Support.Http;
using Haus.Testing.Support;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace Haus.Site.Host.Tests.Support;

public class HausSiteTestContext : TestContext, IAsyncLifetime
{
    private readonly InMemoryHttpClientFactory _httpClientFactory;
    private readonly ConcurrentBag<object> _capturedActions = [];
    private IDispatcher? _dispatcher = null;

    protected InMemoryHttpMessageHandler HausApiHandler => _httpClientFactory.GetHandler(HausApiClientNames.Default);
    
    protected HausSiteTestContext()
    {
        _httpClientFactory = new InMemoryHttpClientFactory();
        Services.AddMudServices()
            .AddHausApiClient(opts =>
            {
                opts.BaseUrl = ConfigureHttpResponseOptions.DefaultBaseUrl;
            });
        Services.AddFluxor(scan => scan.ScanAssemblies(typeof(ListState<>).Assembly));
        
        Services.AddSingleton(_httpClientFactory);
        Services.Replace<IHttpClientFactory>(_httpClientFactory);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    public T[] GetCapturedActions<T>()
    {
        return _capturedActions.OfType<T>().ToArray();
    }
    
    public async Task InitializeAsync()
    {
        RenderComponent<MudPopoverProvider>();
        RenderComponent<MudSnackbarProvider>();
        _dispatcher = Services.GetRequiredService<IDispatcher>();
        _dispatcher.ActionDispatched += CaptureDispatchedAction;
        
        var store = Services.GetRequiredService<IStore>();
        await store.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        if (_dispatcher != null) _dispatcher.ActionDispatched -= CaptureDispatchedAction;
        
        return Task.CompletedTask;
    }

    private void CaptureDispatchedAction(object? sender, ActionDispatchedEventArgs args)
    {
        _capturedActions.Add(args.Action);
    }
}