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

    public async Task<DeviceDetailPage> NavigateToDeviceAsync(string externalId)
    {
        // A freshly-joined, never-renamed device's Name defaults to its ExternalId, so the device
        // list -- which only renders Name, not ExternalId -- can still be searched by it.
        await GetDeviceListItem(externalId).ClickAsync();
        return new DeviceDetailPage(page);
    }

    public Task ReloadAsync()
    {
        return page.GotoAsync("/devices");
    }

    public ILocator GetDeviceListItem(string externalId)
    {
        return page.GetByText(externalId, new PageGetByTextOptions { Exact = true });
    }
}
