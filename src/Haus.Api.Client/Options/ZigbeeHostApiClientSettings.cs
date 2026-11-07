using Haus.Api.Client.Common;

namespace Haus.Api.Client.Options;

public class ZigbeeHostApiClientSettings : IHasBaseUrl
{
    public string? BaseUrl { get; set; }
}
