using System.Text.Json.Serialization;

namespace Haus.Core.Models.Devices;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LightType
{
    None,
    Level,
    Temperature,
    Color
}