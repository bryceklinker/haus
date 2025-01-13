using System.Net.Http;
using Haus.Api.Client;
using Haus.Site.Host.Tests.Support.Http;
using Haus.Testing.Support;
using MudBlazor.Services;

namespace Haus.Site.Host.Tests.Support;

public class HausSiteTestContext : TestContext
{
    protected HausSiteTestContext()
    {
        Services.AddMudServices()
            .AddHausApiClient(opts =>
            {
                opts.BaseUrl = ConfigureHttpResponseOptions.DefaultBaseUrl;
            })
            .Replace<IHttpClientFactory>(new InMemoryHttpClientFactory());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}