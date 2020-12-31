namespace Haus.Core.Models.ExternalMessages
{
    public record HausEvent(string Type = null);

    public record HausEvent<T>(string Type = null, T Payload = default) : HausEvent(Type);
}