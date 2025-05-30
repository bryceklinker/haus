using System.Threading.Tasks;
using FluentAssertions;
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

        settings?.Auth.Domain.Should().Be(_configuration["Auth:Domain"]);
        settings?.Auth.ClientId.Should().Be(_configuration["Auth:ClientId"]);
        settings?.Auth.Audience.Should().Be(_configuration["Auth:Audience"]);
    }

    [Fact]
    public async Task WhenGettingClientSettingsThenReturnsVersion()
    {
        var version = typeof(Startup).Assembly.GetName().Version;
        var expected = $"{version?.Major}.{version?.Minor}.{version?.Build}";

        var settings = await _client.GetClientSettingsAsync();

        settings?.Version.Should().Be(expected);
    }
}
