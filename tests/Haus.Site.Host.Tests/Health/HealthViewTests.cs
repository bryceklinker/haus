using Haus.Core.Models;
using Haus.Site.Host.Health;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;

namespace Haus.Site.Host.Tests.Health;

public class HealthViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsCurrentHealth()
    {
        var hub = GetSubscriber(HausSignalRHubNames.Health);

        RenderView<HealthView>();

        Eventually.Assert(() =>
        {
            hub.IsStarted.Should().BeTrue();
        });
    }
}
