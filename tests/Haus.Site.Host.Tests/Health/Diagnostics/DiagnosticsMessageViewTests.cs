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
    private const string ReplayUrl = "/api/diagnostics/replay";

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

        Assert.NotNull(message.Id);
        Assert.Contains(message.Id, view.Markup);
        Assert.NotNull(message.Topic);
        Assert.Contains(message.Topic, view.Markup);
        Assert.Contains("2023-03-26", view.Markup);
        Assert.Contains("three", view.Markup);
    }

    [Fact]
    public async Task WhenMessageIsReplayingThenReplayIsDisabled()
    {
        await HausApiHandler.SetupPostAsJson(ReplayUrl, new { }, opts => opts.WithDelayMs(500));

        var message = HausModelFactory.MqttDiagnosticsMessageModel();

        var view = RenderView<DiagnosticsMessageView>(opts =>
        {
            opts.Add(c => c.Message, message);
        });

        var invokeTask = view.InvokeAsync(async () =>
        {
            await view.FindByComponent<MudButton>().ClickAsync();
        });

        Eventually.Assert(() =>
        {
            Assert.True(view.FindByComponent<MudButton>().Instance.Disabled);
        });
        await invokeTask;
    }

    [Fact]
    public async Task WhenMessageIsReplayedThenSendsMessageToApi()
    {
        HttpRequestMessage? req = null;
        await HausApiHandler.SetupPostAsJson(ReplayUrl, new { }, opts => opts.WithCapture(r => req = r));
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

            Assert.Equivalent(message, content);
        });
    }
}
