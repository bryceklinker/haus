using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models.Application;
using Haus.Site.Host.Shell;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;

namespace Haus.Site.Host.Tests.Shell;

public class ShellDrawerViewTests : HausSiteTestContext
{
    private const string SettingsUrl = "/api/application/settings";

    [Fact]
    public async Task WhenDeviceSimulatorIsEnabledThenShowsNavigationLinkForIt()
    {
        await HausApiHandler.SetupGetAsJson(SettingsUrl, new ApplicationSettingsModel(IsDeviceSimulatorEnabled: true));

        var view = RenderView<ShellDrawerView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(5, view.FindAllByComponent<MudNavLink>().Count());
        });
    }

    [Fact]
    public async Task WhenDeviceSimulatorIsDisabledThenHidesNavigationLinkForIt()
    {
        await HausApiHandler.SetupGetAsJson(SettingsUrl, new ApplicationSettingsModel(IsDeviceSimulatorEnabled: false));

        var view = RenderView<ShellDrawerView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(4, view.FindAllByComponent<MudNavLink>().Count());
        });
    }

    [Fact]
    public void WhenAccessTokenIsNotYetAvailableThenDoesNotThrowUnhandledException()
    {
        var interactiveRequestOptions = new InteractiveRequestOptions
        {
            Interaction = InteractionType.SignIn,
            ReturnUrl = "authentication/login",
        };
        var accessTokenResult = new AccessTokenResult(
            AccessTokenResultStatus.RequiresRedirect,
            null!,
            "authentication/login",
            interactiveRequestOptions
        );
        var exception = new AccessTokenNotAvailableException(NavigationManager, accessTokenResult, null);
        HausApiHandler.SetupGetThrows(SettingsUrl, exception);

        var view = RenderView<ShellDrawerView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(4, view.FindAllByComponent<MudNavLink>().Count());
        });
    }

    [Fact]
    public async Task WhenAccessTokenIsNotYetAvailableThenDoesNotNavigateAway()
    {
        var interactiveRequestOptions = new InteractiveRequestOptions
        {
            Interaction = InteractionType.SignIn,
            ReturnUrl = "authentication/login",
        };
        var accessTokenResult = new AccessTokenResult(
            AccessTokenResultStatus.RequiresRedirect,
            null!,
            "authentication/login",
            interactiveRequestOptions
        );
        var exception = new AccessTokenNotAvailableException(NavigationManager, accessTokenResult, null);
        HausApiHandler.SetupGetThrows(SettingsUrl, exception);

        RenderView<ShellDrawerView>();

        await Task.Delay(500);

        Assert.Empty(NavigationManager.History);
    }
}
