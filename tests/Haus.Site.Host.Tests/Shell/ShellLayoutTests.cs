using System.Threading.Tasks;
using Haus.Site.Host.Shell;
using Haus.Site.Host.Tests.Support;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.Shell;

public class ShellLayoutTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenMenuIsToggledThenMenuIsClosed()
    {
        var shell = Context.RenderComponent<ShellLayout>();

        await shell.FindByTag("button").ClickAsync(new MouseEventArgs());

        shell.FindAllByClass("mud-drawer--closed").Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenMenuIsToggledTwiceThenMenuIsOpen()
    {
        var shell = Context.RenderComponent<ShellLayout>();

        await shell.FindByTag("button").ClickAsync(new MouseEventArgs());
        await shell.FindByTag("button").ClickAsync(new MouseEventArgs());

        shell.FindAllByClass("mud-drawer--open").Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenThemeModeIsToggledTrueThenThemeIsDark()
    {
        var shell = Context.RenderComponent<ShellLayout>();

        await shell.InvokeAsync(async () =>
        {
            await shell.FindComponent<MudSwitch<bool>>().Instance.ValueChanged.InvokeAsync(true);
        });

        shell.Markup.Should().Contain("--mud-native-html-color-scheme: dark");
    }
    
    [Fact]
    public async Task WhenThemeModeIsToggledThenThemeIsLight()
    {
        var shell = Context.RenderComponent<ShellLayout>();

        await shell.InvokeAsync(async () =>
        {
            await shell.FindComponent<MudSwitch<bool>>().Instance.ValueChanged.InvokeAsync(false);
        });

        shell.Markup.Should().Contain("--mud-native-html-color-scheme: light");
    }
}