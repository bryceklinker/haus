using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support.Pages;

public class ZigbeePage(IPage page)
{
    public async Task NavigateAsync()
    {
        await page.ClickLinkAsync("Zigbee");
    }

    public ILocator GetConnectionStatus()
    {
        return page.GetByText("Connected", new PageGetByTextOptions { Exact = true });
    }

    // Device panel titles render the bare IEEE address as an h6 (see ZigbeeDeviceView); activity
    // entry titles always append " (timestamp)", so an exact h6 match can't collide with them.
    public ILocator GetDeviceListItem(string ieeeAddress)
    {
        return page.GetByRole(
            AriaRole.Heading,
            new PageGetByRoleOptions
            {
                Level = 6,
                Name = ieeeAddress,
                Exact = true,
            }
        );
    }

    // Scoped to the activity feed's own scroll container (see ZigbeeActivityView) so this can't
    // match the devices list' identical IEEE address heading. .First tolerates the address
    // appearing in more than one activity entry (e.g. both device_joined and
    // device_info_discovered carry it) -- any match proves a live entry arrived, since this page
    // is never reloaded between navigating and joining the device.
    public ILocator GetActivityEntryContaining(string ieeeAddress)
    {
        return page.CssLocator("div[style*='max-height: 400px']").GetByText(ieeeAddress).First;
    }
}
