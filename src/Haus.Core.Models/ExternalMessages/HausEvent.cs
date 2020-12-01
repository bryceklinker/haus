namespace Haus.Core.Models.ExternalMessages
{
    public class HausEvent
    {
        public string Type { get; set; }

        public HausEvent(string type = null)
        {
            Type = type;
        }
    }

    public class HausEvent<T> : HausEvent
    {
        public T Payload { get; set; }

        public HausEvent(string type = null, T payload = default)
            : base(type)
        {
            Payload = payload;
        }
    }
}