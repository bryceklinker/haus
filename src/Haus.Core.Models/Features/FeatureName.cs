using System.Text.Json.Serialization;

namespace Haus.Core.Models.Features
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FeatureName
    {
        DeviceSimulator
    }
}