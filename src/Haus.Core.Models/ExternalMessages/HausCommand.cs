namespace Haus.Core.Models.ExternalMessages
{
    public class HausCommand
    {
        public string Type { get; set; }

        public HausCommand(string type = null)
        {
            Type = type;
        }
    }

    public class HausCommand<T> : HausCommand
    {
        public T Payload { get; set; }

        public HausCommand(string type = null, T payload = default)
            : base(type)
        {
            Payload = payload;
        }
    }
}