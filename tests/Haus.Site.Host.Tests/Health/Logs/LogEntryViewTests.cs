using System.Linq;
using Haus.Core.Models.Logs;
using Haus.Site.Host.Health.Logs;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Haus.Site.Host.Tests.Health.Logs;

public class LogEntryViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsLogMessage()
    {
        var logEntry = HausModelFactory.LogEntryModel();

        var view = RenderLogView(logEntry);

        Assert.Contains(logEntry.Level, view.Markup);
        Assert.Contains(logEntry.Timestamp, view.Markup);
        Assert.Contains(logEntry.Message, view.Markup);
    }

    [Theory]
    [InlineData(LogLevel.Debug, Icons.Material.Filled.BugReport)]
    [InlineData(LogLevel.Information, Icons.Material.Filled.Info)]
    [InlineData(LogLevel.Warning, Icons.Material.Filled.Warning)]
    [InlineData(LogLevel.Error, Icons.Material.Filled.Error)]
    [InlineData(LogLevel.Critical, Icons.Material.Filled.Sick)]
    public void WhenRenderedWithLogLevelThenShowsIcon(LogLevel level, string icon)
    {
        var logEntry = HausModelFactory.LogEntryModel() with { Level = $"{level}" };

        var view = RenderLogView(logEntry);

        Assert.Equal(icon, view.FindByComponent<MudIcon>().Instance.Icon);
    }

    private IRenderedComponent<LogEntryView> RenderLogView(LogEntryModel model)
    {
        return RenderView<LogEntryView>(opts =>
        {
            opts.Add(c => c.LogEntry, model);
        });
    }
}
