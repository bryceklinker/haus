using Haus.Api.Client.Common;

namespace Haus.Api.Client.Options;

public class HausApiClientSettings : IHasBaseUrl
{
    public string? BaseUrl { get; set; }
}
