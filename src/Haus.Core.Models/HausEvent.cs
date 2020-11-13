using System.Text.Json;

namespace Haus.Core.Models
{
    public class HausEvent
    {
        public string Type { get; set; }
        public string Payload { get; set; }

        public T GetPayload<T>()
        {
            return JsonSerializer.Deserialize<T>(Payload);
        }
    }
}