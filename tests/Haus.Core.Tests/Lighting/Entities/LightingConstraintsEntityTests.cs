using FluentAssertions;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.Lighting.Entities
{
    public class LightingConstraintsEntityTests
    {
        [Fact]
        public void WhenUpdatedFromModelThenConstraintsMatchModel()
        {
            var entity = new DefaultLightingConstraintsEntity();
            entity.UpdateFromModel(new LightingConstraintsModel(54, 65, 9000, 10000));

            entity.Constraints.MinLevel.Should().Be(54);
            entity.Constraints.MaxLevel.Should().Be(65);
            entity.Constraints.MinTemperature.Should().Be(9000);
            entity.Constraints.MaxTemperature.Should().Be(10000);
        }

        [Fact]
        public void WhenConvertedToModelThenReturnsModelPopulatedFromEntity()
        {
            var entity = new LightingConstraintsEntity(23, 45, 100, 200);

            var model = entity.ToModel();

            model.MinLevel.Should().Be(23);
            model.MaxLevel.Should().Be(45);
            model.MinTemperature.Should().Be(100);
            model.MaxTemperature.Should().Be(200);
        }
    }
}