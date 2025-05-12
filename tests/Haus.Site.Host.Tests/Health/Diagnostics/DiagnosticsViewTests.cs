using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Diagnostics;
using Haus.Site.Host.Health.Diagnostics;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Health.Diagnostics;

public class DiagnosticsViewTests : HausSiteTestContext
{
    private readonly InMemoryRealtimeDataSubscriber _diagnosticsSubscriber;

    public DiagnosticsViewTests()
    {
        _diagnosticsSubscriber = GetSubscriber(HausRealtimeSources.Diagnostics);
    }

    [Fact]
    public void WhenRenderedThenStartsRealtimeDiagnostics()
    {
        RenderView<DiagnosticsView>();

        Eventually.Assert(() =>
        {
            _diagnosticsSubscriber.IsStarted.Should().BeTrue();
        });
    }

    [Fact]
    public void WhenConnectingToHubTakesAwhileThenShowsLoading()
    {
        _diagnosticsSubscriber.ConfigureStartDelayMs(1000);

        var view = RenderView<DiagnosticsView>();
        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudProgressCircular>().Should().HaveCount(1);
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsDiagnosticEvents()
    {
        var view = RenderView<DiagnosticsView>();
        await _diagnosticsSubscriber.SimulateAsync(
            DiagnosticsEventNames.MqttMessage,
            HausModelFactory.MqttDiagnosticsMessageModel()
        );
        await _diagnosticsSubscriber.SimulateAsync(
            DiagnosticsEventNames.MqttMessage,
            HausModelFactory.MqttDiagnosticsMessageModel()
        );
        await _diagnosticsSubscriber.SimulateAsync(
            DiagnosticsEventNames.MqttMessage,
            HausModelFactory.MqttDiagnosticsMessageModel()
        );

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<DiagnosticsMessageView>().Should().HaveCount(3);
        });
    }
}
