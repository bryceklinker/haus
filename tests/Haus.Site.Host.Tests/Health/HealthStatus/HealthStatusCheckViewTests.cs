using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Haus.Site.Host.Health.HealthStatus;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;
using MudBlazor.Utilities;

namespace Haus.Site.Host.Tests.Health.HealthStatus;

public class HealthStatusCheckViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenHealthCheckRenderedThenShowsDetails()
    {
        var check = HausModelFactory.HealthCheckModel() with
        {
            Description = "hello",
            DurationOfCheckInMilliseconds = 9000,
            ExceptionMessage = "bad things",
        };
        var view = RenderView<HealthStatusCheckView>(ops =>
        {
            ops.Add(p => p.Check, check);
        });

        Eventually.Assert(() =>
        {
            view.FindMudTextFieldById<string>("description").GetValue().Should().Be("hello");
            view.FindMudTextFieldById<double?>("duration").GetValue().Should().Be(9);
            view.FindMudTextFieldById<string>("error").GetValue().Should().Be("bad things");
        });
    }

    [Theory]
    [InlineData(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy, "#4caf50ff")]
    [InlineData(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy, "#f44336ff")]
    [InlineData(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded, "#ff5722ff")]
    public void WhenCheckHasStatusThenPanelIsInCorrectColor(
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus status,
        string color
    )
    {
        var check = HausModelFactory.HealthCheckModel() with { Status = status };
        var view = RenderView<HealthStatusCheckView>(ops =>
        {
            ops.Add(p => p.Check, check);
        });

        Eventually.Assert(() =>
        {
            var panel = view.FindByComponent<MudExpansionPanel>();
            var title = panel.FindByComponent<MudPaper>(opts => opts.WithText(check.Name));
            title.Instance.Style.Should().Contain($"background-color: {color}");
        });
    }
}
