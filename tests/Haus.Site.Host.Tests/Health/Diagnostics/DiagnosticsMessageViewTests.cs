using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Diagnostics;
using Haus.Site.Host.Health.Diagnostics;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Health.Diagnostics;

public class DiagnosticsMessageViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsMessageContent()
    {
        var message = HausModelFactory.MqttDiagnosticsMessageModel() with
        {
            Timestamp = DateTime.Parse("2023-03-26T12:00:00.000Z"),
            Payload = new { id = "three" },
        };
        var view = RenderView<DiagnosticsMessageView>(opts =>
        {
            opts.Add(c => c.Message, message);
        });

        view.Markup.Should().Contain(message.Id);
        view.Markup.Should().Contain(message.Topic);
        view.Markup.Should().Contain("2023-03-26");
        view.Markup.Should().Contain("three");
    }

    [Fact]
    public async Task WhenMessageIsReplayingThenReplayIsDisabled()
    {
        await HausApiHandler.SetupPostAsJson("/api/diagnostics/replay", new { }, opts => opts.WithDelayMs(1000));

        var message = HausModelFactory.MqttDiagnosticsMessageModel();

        var view = RenderView<DiagnosticsMessageView>(opts =>
        {
            opts.Add(c => c.Message, message);
        });

        await view.FindByComponent<MudButton>().ClickAsync();

        Eventually.Assert(() =>
        {
            view.FindByComponent<MudButton>().Instance.Disabled.Should().BeTrue();
        });
    }

    [Fact]
    public async Task WhenMessageIsReplayedThenSendsMessageToApi()
    {
        HttpRequestMessage? req = null;
        await HausApiHandler.SetupPostAsJson(
            "/api/diagnostics/replay",
            new { },
            opts => opts.WithCapture(r => req = r)
        );
        var message = HausModelFactory.MqttDiagnosticsMessageModel();

        var view = RenderView<DiagnosticsMessageView>(opts =>
        {
            opts.Add(c => c.Message, message);
        });

        await view.FindByComponent<MudButton>().ClickAsync();

        await Eventually.AssertAsync(async () =>
        {
            var content =
                req?.Content != null ? await req.Content.ReadFromJsonAsync<MqttDiagnosticsMessageModel>() : null;

            content.Should().BeEquivalentTo(message);
        });
    }
}
