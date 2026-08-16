using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Application;

[Collection(HausWebHostCollectionFixture.Name)]
public class ApplicationSettingsApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task WhenGettingApplicationSettingsThenReturnsDeviceSimulatorEnabledState()
    {
        var settings = await _client.GetSettingsAsync();

        Assert.NotNull(settings);
        Assert.True(settings.IsDeviceSimulatorEnabled);
    }
}
