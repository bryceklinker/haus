using System.Collections.Generic;
using Haus.Api.Client.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Haus.Site.Host.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void WhenBrowserHostDiffersFromConfiguredApiHostThenRegisteredApiClientUsesBrowserHost()
    {
        var services = new ServiceCollection();

        services.AddHausSiteServices(BuildConfiguration(), "https://192.168.1.50:5003/");

        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<HausApiClientSettings>>().Value;
        Assert.Equal("https://192.168.1.50:5001", settings.BaseUrl);
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Api:BaseUrl"] = "https://localhost:5001",
                    ["Auth:Domain"] = "haus-app.us.auth0.com",
                    ["Auth:ClientId"] = "client-id",
                    ["Auth:Audience"] = "https://haus-portal-api.com",
                }
            )
            .Build();
    }
}
