using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support.Pages;

public class DevicesPage(IPage page)
{
    public async Task NavigateAsync()
    {
        await page.ClickLinkAsync("Devices");
    }

    public async Task<DevicesDiscoveryPage> NavigateToDiscoveryAsync()
    {
        await NavigateAsync();
        var discovery = new DevicesDiscoveryPage(page);
        await discovery.NavigateAsync();
        return discovery;
    }
}
