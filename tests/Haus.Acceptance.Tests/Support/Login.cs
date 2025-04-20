using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support;

public static class LoginExtensions
{
    public static async Task PerformLoginAsync(this IPage page, HausUser user)
    {
        await page.GotoAsync("http://localhost:5002/welcome");
        await page.ClickButtonAsync("Login");

        await page.EnterTextAsync("email", user.Email);
        await page.EnterTextAsync("password", user.Password);
        await page.ClickButtonAsync("Log In");
    }
}
