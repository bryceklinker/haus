using System.Text.Json;

namespace Haus.Core.Models
{
    public static class HausJsonSerializer
    {
        public static bool TryDeserialize<T>(byte[] bytes, out T value)
        {
            try
            {
                value = Deserialize<T>(bytes);
                return true;
            }
            catch (JsonException)
            {
                value = default;
                return false;
            }
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes);
        }

        public static byte[] SerializeToBytes(object value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value);
        }
    }
}