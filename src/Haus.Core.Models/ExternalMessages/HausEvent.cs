namespace Haus.Core.Models.ExternalMessages
{
    [SkipGeneration]
    public record HausEvent(string Type = null);

    [SkipGeneration]
    public record HausEvent<T>(string Type = null, T Payload = default) : HausEvent(Type);
}