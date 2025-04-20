using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support;

public static class PageExtensions
{
    public static async Task ClickButtonAsync(this IPage page, string name, int index = 0)
    {
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = name }).Nth(index).ClickAsync();
    }

    public static async Task ClickLinkAsync(this IPage page, string name, int index = 0)
    {
        await page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = name }).Nth(index).ClickAsync();
    }

    public static async Task EnterTextAsync(this IPage page, string label, string value, int index = 0)
    {
        await page.GetByLabel(label).Nth(index).FillAsync(value);
    }
}
