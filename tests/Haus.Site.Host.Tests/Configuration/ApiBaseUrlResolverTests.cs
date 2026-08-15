using Haus.Site.Host.Configuration;

namespace Haus.Site.Host.Tests.Configuration;

public class ApiBaseUrlResolverTests
{
    [Fact]
    public void WhenBrowserHostDiffersFromConfiguredHostThenResolvesUsingBrowserHost()
    {
        var resolved = ApiBaseUrlResolver.Resolve("https://localhost:5001", "https://192.168.1.50:5003/");

        Assert.Equal("https://192.168.1.50:5001", resolved);
    }

    [Fact]
    public void WhenBrowserHostMatchesConfiguredHostThenResolvedUrlIsUnchanged()
    {
        var resolved = ApiBaseUrlResolver.Resolve("https://localhost:5001", "https://localhost:5003/");

        Assert.Equal("https://localhost:5001", resolved);
    }
}
