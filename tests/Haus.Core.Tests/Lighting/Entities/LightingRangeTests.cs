using System;
using FluentAssertions;
using Haus.Core.Lighting.Entities;
using Xunit;

namespace Haus.Core.Tests.Lighting.Entities;

public class LightingRangeTests
{
    [Fact]
    public void WhenTargetValueIsCalculatedWithNullThenThrowsException()
    {
        Action act = () => new LevelLightingEntity().CalculateTargetValue(null);

        act.Should().Throw<ArgumentNullException>();
    }
}