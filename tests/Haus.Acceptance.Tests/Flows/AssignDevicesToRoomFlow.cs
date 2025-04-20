using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Microsoft.Playwright.NUnit;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class AssignDevicesToRoomFlow : PageTest
{
    [Test]
    public async Task AssignDeviceToRoom()
    {
        await Page.PerformLoginAsync(HausUser.Default);

        await Page.ClickLinkAsync("Devices");
        await Page.ClickLinkAsync("Discovery");

        await Expect(Page).ToHaveTitleAsync("Haus.Site.Host");
    }
}
