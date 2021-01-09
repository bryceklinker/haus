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
    }
}