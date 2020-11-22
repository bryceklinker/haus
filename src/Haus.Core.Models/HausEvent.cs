namespace Haus.Core.Models
{
    public class HausEvent
    {
        public string Type { get; set; }
    }

    public class HausEvent<T> : HausEvent
    {
        public T Payload { get; set; }
    }
}