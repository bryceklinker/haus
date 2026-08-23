using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Zigbee.Activity;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Zigbee.Activity;

public class ZigbeeActivityEntryViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsEventTypeAndTimestamp()
    {
        var entry = HausModelFactory.ZigbeeActivityEntryModel() with { EventType = "zigbee_device_joined" };

        var view = RenderView<ZigbeeActivityEntryView>(opts => opts.Add(p => p.Entry, entry));

        Eventually.Assert(() =>
        {
            var panel = view.FindByComponent<MudExpansionPanel>();
            var title = panel.FindByComponent<MudText>(opts => opts.WithText("zigbee_device_joined"));
            Assert.NotNull(title);
        });
    }
}
