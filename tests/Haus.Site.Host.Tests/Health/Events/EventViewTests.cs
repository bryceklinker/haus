using Haus.Core.Models.ExternalMessages;
using Haus.Site.Host.Health.Events;
using Haus.Site.Host.Tests.Support;

namespace Haus.Site.Host.Tests.Health.Events;

public class EventViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedWithEventThenShowsEventInformation()
    {
        var hausEvent = new HausEvent<dynamic>("Hello", new { Name = "Bob" }, "2025-02-12");
        var view = RenderEventView(hausEvent);

        Assert.Contains("Hello", view.Markup);
        Assert.Contains("Bob", view.Markup);
        Assert.Contains("2025-02-12", view.Markup);
    }

    private IRenderedComponent<EventView> RenderEventView(HausEvent<dynamic> @event)
    {
        return RenderView<EventView>(opts =>
        {
            opts.Add(c => c.Event, @event);
        });
    }
}
