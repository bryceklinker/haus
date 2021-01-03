using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.Lighting;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Lighting
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class LightingApiTests
    {
        private readonly IHausApiClient _client;

        public LightingApiTests(HausWebHostApplicationFactory factory)
        {
            _client = factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenGettingDefaultLightingConstraintsThenReturnsDefaultLightingConstraints()
        {
            var constraints = await _client.GetDefaultLightingConstraintsAsync();

            constraints.Should().BeEquivalentTo(new LightingConstraintsModel());
        }

        [Fact]
        public async Task WhenDefaultLightingConstraintsAreUpdatedThenNewDefaultsAreAvailable()
        {
            await _client.UpdateDefaultLightingConstraintsAsync(new LightingConstraintsModel(12, 95, 123, 129));

            var constraints = await _client.GetDefaultLightingConstraintsAsync();
            constraints.Should().BeEquivalentTo(new LightingConstraintsModel(12, 95, 123, 129));
        }
    }
}