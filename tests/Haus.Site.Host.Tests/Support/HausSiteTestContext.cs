using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Site.Host.Tests.Support.Http;
using Haus.Testing.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

namespace Haus.Site.Host.Tests.Support;

public class HausSiteTestContext : IAsyncLifetime
{
    private readonly InMemoryHttpClientFactory _httpClientFactory;
    protected TestContext Context { get; }
    protected InMemoryHttpMessageHandler HausApiHandler => _httpClientFactory.GetHandler(HausApiClientNames.Default);

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
}
