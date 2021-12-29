using FluentAssertions;
using Haus.Core.Discovery.Entities;
using Haus.Core.Models.Discovery;
using Xunit;

namespace Haus.Core.Tests.Discovery.Entities;

public class DiscoveryEntityTests
{
    [Fact]
    public void WhenDiscoveryStartedThenDiscoveryIsEnabled()
    {
        var entity = new DiscoveryEntity();

        entity.Start();

        entity.State.Should().Be(DiscoveryState.Enabled);
    }

    [Fact]
    public void WhenDiscoveryStoppedThenDiscoveryIsDisabled()
    {
        var entity = new DiscoveryEntity();

        entity.Start();
        entity.Stop();

        entity.State.Should().Be(DiscoveryState.Disabled);
    }
}