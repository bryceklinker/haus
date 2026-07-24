using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.ExternalMessages;
using Haus.Site.Host.Health.Events;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Health.Events;

public class EventsViewTests : HausSiteTestContext
{
    private readonly InMemoryRealtimeDataSubscriber _eventsSubscriber;

    public EventsViewTests()
    {
        _eventsSubscriber = GetSubscriber(HausRealtimeSources.Events);
    }

    [Fact]
    public void WhenRenderedThenStartsEventSubscriber()
    {
        RenderView<EventsView>();

        Eventually.Assert(() =>
        {
            Assert.True(_eventsSubscriber.IsStarted);
        });
    }

    [Fact]
    public void WhenConnectingToHubThenLoadingIsShown()
    {
        _eventsSubscriber.ConfigureStartDelayMs(500);

        var view = RenderView<EventsView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(1, view.FindAllByComponent<MudProgressCircular>().Count());
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsRealtimeEvents()
    {
        var view = RenderView<EventsView>();
        await _eventsSubscriber.SimulateAsync(HausEventsEventNames.OnEvent, new HausEvent<object>("IDK", new { }));

        Eventually.Assert(() =>
        {
            Assert.Equal(1, view.FindAllByComponent<MudExpansionPanels>().Count());
            Assert.Equal(1, view.FindAllByComponent<MudExpansionPanel>().Count());
        });
    }
}
