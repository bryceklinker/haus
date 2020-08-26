using System.Text.Json;

namespace Haus.ServiceBus.Common
{
    public class ServiceBusMessage<TPayload>
    {
        public string Type { get; set; }
        public TPayload Payload { get; set; }

        public ServiceBusMessage(string type, TPayload payload)
        {
            Type = type;
            Payload = payload;
        }

        public byte[] ToBytes()
        {
            return JsonSerializer.SerializeToUtf8Bytes(this);
        }

        public static ServiceBusMessage<TPayload> FromBytes(byte[] bytes)
        {
            return JsonSerializer.Deserialize<ServiceBusMessage<TPayload>>(bytes);
        }
    }
}