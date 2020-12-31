namespace Haus.Core.Models.ExternalMessages
{
    public record HausCommand(string Type = null);

    public record HausCommand<T>(string Type = null, T Payload = default) : HausCommand(Type);
}