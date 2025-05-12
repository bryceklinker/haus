using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Http;

public record ConfigureHttpResponseOptions(
    TimeSpan Delay = default,
    string BaseUrl = ConfigureHttpResponseOptions.DefaultBaseUrl,
    Func<HttpRequestMessage, Task>? Capture = null
)
{
    public Uri BaseUri => new(BaseUrl);
    public const string DefaultBaseUrl = "https://localhost:5000";
    public Func<HttpRequestMessage, Task> Capture { get; protected init; } = Capture ?? (_ => Task.CompletedTask);
}

public record ConfigureHttpResponseWithStatus(
    HttpStatusCode Status = HttpStatusCode.OK,
    TimeSpan Delay = default,
    Func<HttpRequestMessage, Task>? Capture = null
) : ConfigureHttpResponseOptions(Delay: Delay, Capture: Capture)
{
    public ConfigureHttpResponseWithStatus WithStatus(HttpStatusCode status)
    {
        return this with { Status = status };
    }

    public ConfigureHttpResponseWithStatus WithDelayMs(long delay)
    {
        return WithDelay(TimeSpan.FromMilliseconds(delay));
    }

    public ConfigureHttpResponseWithStatus WithDelay(TimeSpan delay)
    {
        return this with { Delay = delay };
    }

    public ConfigureHttpResponseWithStatus WithCapture(Func<HttpRequestMessage, Task> capture)
    {
        return this with { Capture = capture };
    }

    public ConfigureHttpResponseWithStatus WithCapture(Action<HttpRequestMessage> capture)
    {
        return this with
        {
            Capture = req =>
            {
                capture(req);
                return Task.CompletedTask;
            },
        };
    }
}
