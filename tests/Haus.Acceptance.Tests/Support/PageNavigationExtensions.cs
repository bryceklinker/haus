using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support.Pages;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support;

public static class PageNavigationExtensions
{
    public static async Task<DevicesPage> NavigateToDevicesAsync(this IPage page)
    {
        var devicesPage = new DevicesPage(page);
        await devicesPage.NavigateAsync();
        return devicesPage;
    }

    public static async Task<RoomsPage> NavigateToRoomsAsync(this IPage page)
    {
        var roomsPage = new RoomsPage(page);
        await roomsPage.NavigateAsync();
        return roomsPage;
    }

    public static async Task<ZigbeePage> NavigateToZigbeeAsync(this IPage page)
    {
        var zigbeePage = new ZigbeePage(page);
        await zigbeePage.NavigateAsync();
        return zigbeePage;
    }
}
