using System;
using System.Net;

namespace Haus.Site.Host.Tests.Support.Http;

public record ConfigureHttpResponseOptions(
    TimeSpan Delay = default,
    string BaseUrl = ConfigureHttpResponseOptions.DefaultBaseUrl
)
{
    public Uri BaseUri => new(BaseUrl);
    public const string DefaultBaseUrl = "https://localhost:5000";
}

public record ConfigureHttpResponseWithStatus(
    HttpStatusCode Status = HttpStatusCode.OK,
    TimeSpan Delay = default
) : ConfigureHttpResponseOptions(Delay);