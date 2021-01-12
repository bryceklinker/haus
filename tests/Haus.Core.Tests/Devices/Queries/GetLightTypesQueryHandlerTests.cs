using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Devices.Queries;
using Haus.Core.Models.Devices;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Queries
{
    public class GetLightTypesQueryHandlerTests
    {
        private readonly IHausBus _hausBus;

        public GetLightTypesQueryHandlerTests()
        {
            _hausBus = HausBusFactory.Create();
        }

        [Fact]
        public async Task WhenGettingLightTypesThenReturnsExcludesNone()
        {
            var result = await _hausBus.ExecuteQueryAsync(new GetLightTypesQuery());

            result.Items.Should().NotContain(LightType.None);
        }

        [Fact]
        public async Task WhenGettingLightTypesThenReturnsAvailableLightTypes()
        {
            var result = await _hausBus.ExecuteQueryAsync(new GetLightTypesQuery());

            result.Count.Should().Be(3);
            result.Items.Should().HaveCount(3)
                .And.Contain(LightType.Color)
                .And.Contain(LightType.Level)
                .And.Contain(LightType.Temperature);
        }
    }
}