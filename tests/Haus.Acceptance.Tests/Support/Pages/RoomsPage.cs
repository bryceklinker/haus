using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support.Pages;

public class RoomsPage(IPage page)
{
    public async Task NavigateAsync()
    {
        await page.ClickLinkAsync("Rooms");
    }

    public async Task AddRoomAsync(string roomName)
    {
        await page.CssLocator(".mud-fab").ClickAsync();
        await page.EnterTextAsync("name", roomName);
        await page.ClickButtonAsync("Save");
    }
}
