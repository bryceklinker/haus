using System.Text.Json.Serialization;

namespace Haus.Core.Models.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LightingState
    {
        On,
        Off
    }
}