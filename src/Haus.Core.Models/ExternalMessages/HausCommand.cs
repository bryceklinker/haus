namespace Haus.Core.Models.ExternalMessages
{
    [SkipGeneration]
    public record HausCommand(string Type = null);

    [SkipGeneration]
    public record HausCommand<T>(string Type = null, T Payload = default) : HausCommand(Type);
}