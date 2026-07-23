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

        Assert.Equal(DiscoveryState.Enabled, entity.State);
    }

    [Fact]
    public void WhenDiscoveryStoppedThenDiscoveryIsDisabled()
    {
        var entity = new DiscoveryEntity();

        entity.Start();
        entity.Stop();

        Assert.Equal(DiscoveryState.Disabled, entity.State);
    }
}
