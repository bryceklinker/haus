using System.Text.Json.Serialization;

namespace Haus.Core.Models.Discovery
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiscoveryState
    {
        Enabled,
        Disabled
    }
}