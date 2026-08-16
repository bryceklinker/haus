using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support.Pages;

public class DeviceDetailPage(IPage page, long deviceId)
{
    public Task ReloadAsync()
    {
        return page.GotoAsync($"/devices/{deviceId}");
    }

    public async Task DeleteAsync()
    {
        await page.RunAndWaitForResponseAsync(
            async () =>
            {
                await page.CssLocator(".device-detail .mud-icon-button").ClickAsync();
                await page.ClickButtonAsync("Delete");
            },
            response => response.Request.Method == "DELETE" && response.Url.Contains("/api/devices/")
        );
    }
}
