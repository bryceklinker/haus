using System.Threading.Tasks;
using Haus.Site.Host.Shell;
using Haus.Site.Host.Tests.Support;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.Shell;

public class ShellLayoutViewTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenMenuIsToggledThenMenuIsClosed()
    {
        var shell = Context.Render<ShellLayoutView>();

        await shell.FindByTag("button").ClickAsync(new MouseEventArgs());

        Assert.Single(shell.FindAllByClass("mud-drawer--closed"));
    }

    [Fact]
    public async Task WhenMenuIsToggledTwiceThenMenuIsOpen()
    {
        var shell = Context.Render<ShellLayoutView>();

        await shell.FindByTag("button").ClickAsync(new MouseEventArgs());
        await shell.FindByTag("button").ClickAsync(new MouseEventArgs());

        Assert.Single(shell.FindAllByClass("mud-drawer--open"));
    }

    [Fact]
    public async Task WhenThemeModeIsToggledTrueThenThemeIsDark()
    {
        var shell = Context.Render<ShellLayoutView>();

        await shell.InvokeAsync(async () =>
        {
            await shell.FindComponent<MudSwitch<bool>>().Instance.ValueChanged.InvokeAsync(true);
        });

        Assert.Contains("--mud-native-html-color-scheme: dark", shell.Markup);
    }

    [Fact]
    public async Task WhenThemeModeIsToggledThenThemeIsLight()
    {
        var shell = Context.Render<ShellLayoutView>();

        await shell.InvokeAsync(async () =>
        {
            await shell.FindComponent<MudSwitch<bool>>().Instance.ValueChanged.InvokeAsync(false);
        });

        Assert.Contains("--mud-native-html-color-scheme: light", shell.Markup);
    }
}
