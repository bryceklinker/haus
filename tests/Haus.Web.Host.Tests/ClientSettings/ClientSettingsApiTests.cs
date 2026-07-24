using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Web.Host.Tests.Support;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Web.Host.Tests.ClientSettings;

[Collection(HausWebHostCollectionFixture.Name)]
public class ClientSettingsApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateUnauthenticatedClient();
    private readonly IConfiguration _configuration = factory.Services.GetRequiredService<IConfiguration>();

    [Fact]
    public async Task WhenGettingClientSettingsThenReturnsAuthSettings()
    {
        var settings = await _client.GetClientSettingsAsync();

        if (settings != null)
        {
            Assert.Equal(_configuration["Auth:Domain"], settings.Auth.Domain);
            Assert.Equal(_configuration["Auth:ClientId"], settings.Auth.ClientId);
            Assert.Equal(_configuration["Auth:Audience"], settings.Auth.Audience);
        }
    }

    [Fact]
    public async Task WhenGettingClientSettingsThenReturnsVersion()
    {
        var version = typeof(Startup).Assembly.GetName().Version;
        var expected = $"{version?.Major}.{version?.Minor}.{version?.Build}";

        var settings = await _client.GetClientSettingsAsync();

        if (settings != null)
        {
            Assert.Equal(expected, settings.Version);
        }
    }
}
