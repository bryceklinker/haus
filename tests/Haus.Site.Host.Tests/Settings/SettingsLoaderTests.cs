using System;
using System.Net;
using System.Threading.Tasks;
using Haus.Site.Host.Shared.Settings;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Http;

namespace Haus.Site.Host.Tests.Settings;

public class SettingsLoaderTests : HausSiteTestContext
{
    private readonly InMemoryHttpClientFactory _httpClientFactory;
    private readonly SettingsLoader _loader;

    public SettingsLoaderTests()
    {
        _httpClientFactory = new InMemoryHttpClientFactory();
        _loader = new SettingsLoader(_httpClientFactory, new FakeWebAssemblyHostEnvironment
        {
            BaseAddress = ConfigureHttpResponseOptions.DefaultBaseUrl
        });
    }
    
    [Fact]
    public async Task WhenSettingsAreLoadedThenReturnsSettings()
    {
        var handler = _httpClientFactory.GetHandler();
        var settings = new SettingsState(
            new ApiSettingsModel("https://localhost:9000"),
            new AuthSettingsModel("domain", "client-id", "audience")
        );
        await handler.SetupGetAsJson("/settings", settings);
        
        var result = await _loader.LoadAsync();

        result.Should().BeEquivalentTo(settings);
    }

    [Fact]
    public async Task WhenSettingsFailToLoadThenThrowsError()
    {
        var handler = _httpClientFactory.GetHandler();
        await handler.SetupGetAsJson<object?>("/settings", null, new ConfigureHttpResponseWithStatus(Status: HttpStatusCode.NotFound));
        
        await _loader.Awaiting(l => l.LoadAsync()).Should().ThrowAsync<Exception>();
    }
}