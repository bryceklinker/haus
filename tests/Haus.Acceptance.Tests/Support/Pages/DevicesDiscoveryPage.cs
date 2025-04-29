using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support.Pages;

public class DevicesDiscoveryPage(IPage page)
{
    public async Task NavigateAsync()
    {
        await page.ClickLinkAsync("Discovery");
    }

    public async Task AssignDeviceToRoomAsync(string deviceId, string roomName)
    {
        await page.GetByText(deviceId).DragToAsync(page.GetByText(roomName));
    }

    public ILocator GetRoomDropZone(string roomName)
    {
        return page.CssLocator($".mud-drop-zone[room-name~='{roomName}']");
    }

    public ILocator GetUnassignedDevicesDropZone()
    {
        return page.CssLocator(".mud-drop-zone[identifier~='unassigned']");
    }
}
