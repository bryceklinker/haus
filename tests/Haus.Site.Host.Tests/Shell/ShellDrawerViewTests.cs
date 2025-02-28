using Haus.Site.Host.Shell;
using Haus.Site.Host.Tests.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Shell;

public class ShellDrawerViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsNavigationLinks()
    {
        var view = RenderView<ShellDrawerView>();

        view.FindAllByComponent<MudNavLink>().Should().HaveCount(4);
    }
}
