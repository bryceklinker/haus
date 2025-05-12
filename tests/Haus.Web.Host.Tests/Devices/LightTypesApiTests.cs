using System.Threading.Tasks;
using FluentAssertions;
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

        result.Count.Should().Be(3);
        result
            .Items.Should()
            .HaveCount(3)
            .And.Contain(LightType.Color)
            .And.Contain(LightType.Level)
            .And.Contain(LightType.Temperature);
    }
}
