using System;

namespace Haus.Site.Host.Configuration;

public static class ApiBaseUrlResolver
{
    public static string Resolve(string configuredBaseUrl, string hostEnvironmentBaseAddress)
    {
        var configured = new Uri(configuredBaseUrl);
        var browserHost = new Uri(hostEnvironmentBaseAddress).Host;
        return new UriBuilder(configured) { Host = browserHost }.Uri.GetLeftPart(UriPartial.Authority);
    }
}
