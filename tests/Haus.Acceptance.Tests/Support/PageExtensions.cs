using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support.Pages;
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

    public static ILocator CssLocator(this IPage page, string selector, PageLocatorOptions? options = null)
    {
        return page.Locator($"css={selector}", options);
    }

    public static ILocator CssLocatorWithText(this IPage page, string selector, string text)
    {
        return page.CssLocator(
            selector,
            new PageLocatorOptions { HasTextRegex = new Regex(text, RegexOptions.IgnoreCase) }
        );
    }
}
