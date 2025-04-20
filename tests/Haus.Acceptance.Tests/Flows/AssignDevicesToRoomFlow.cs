using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class AssignDevicesToRoomFlow : PageTest
{
    [Test]
    public async Task AssignDeviceToRoom()
    {
        await Page.PerformLoginAsync(HausUser.Default);

        await Expect(Page).ToHaveTitleAsync("Haus.Site.Host");
    }
}
