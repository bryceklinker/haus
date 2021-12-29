using System.Text.Json.Serialization;

namespace Haus.Core.Models.Lighting;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LightingState
{
    On,
    Off
}