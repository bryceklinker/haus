namespace Haus.Core.Models.ExternalMessages
{
    public record HausFeedItem(string Type = null, object Payload = default);
}