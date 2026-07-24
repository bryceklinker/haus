using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models.Devices;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class LightTypesApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task WhenGettingLightTypesThenReturnsAllLightTypes()
    {
        var result = await _client.GetLightTypesAsync();

        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.Items.Length);
        Assert.Contains(LightType.Color, result.Items);
        Assert.Contains(LightType.Level, result.Items);
        Assert.Contains(LightType.Temperature, result.Items);
    }
}
