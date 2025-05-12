using System.Threading.Tasks;
using Haus.Site.Host.Shared.Theming;
using Haus.Site.Host.Shell;
using Haus.Site.Host.Tests.Support;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.Shell;

public class ShellHeaderViewTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenMenuClickedThenNotifiesMenuToggled()
    {
        var toggled = false;
        var header = Context.RenderComponent<ShellHeaderView>(p => p.Add(h => h.OnMenuToggled, () => toggled = true));

        await header.FindByTag("button").ClickAsync(new MouseEventArgs());

        toggled.Should().BeTrue();
    }

    [Fact]
    public async Task WhenThemeToggleClickedThenNotifiesThemeToggled()
    {
        var themeMode = ThemeMode.Light;

        var header = Context.RenderComponent<ShellHeaderView>(p =>
            p.Add(h => h.OnThemeToggled, (mode) => themeMode = mode)
        );

        await header.InvokeAsync(async () =>
        {
            await header.FindComponent<MudSwitch<bool>>().Instance.ValueChanged.InvokeAsync(true);
        });

        themeMode.Should().Be(ThemeMode.Dark);
    }
}
