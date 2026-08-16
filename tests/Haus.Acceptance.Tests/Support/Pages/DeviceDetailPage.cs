using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support.Pages;

public class DeviceDetailPage(IPage page)
{
    private readonly string _detailUrl = page.Url;

    public Task ReloadAsync()
    {
        return page.GotoAsync(_detailUrl);
    }

    public async Task DeleteAsync()
    {
        await page.CssLocator(".device-detail .mud-icon-button").ClickAsync();
        var dialogTitle = page.GetByText("Delete Device");
        await page.ClickButtonAsync("Delete");
        await dialogTitle.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden });
    }
}
