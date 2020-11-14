using System.Text.Json;

namespace Haus.Core.Models
{
    public class HausEvent
    {
        public string Type { get; set; }
        public string Payload { get; set; }
        public bool HasPayload => !string.IsNullOrWhiteSpace(Payload);
        public T GetPayload<T>()
        {
            return HasPayload
                ? JsonSerializer.Deserialize<T>(Payload)
                : default;
        }
    }
}