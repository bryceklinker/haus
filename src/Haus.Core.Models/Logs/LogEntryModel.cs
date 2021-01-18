namespace Haus.Core.Models.Logs
{
    public record LogEntryModel(
        string Timestamp,
        string Level,
        string Message,
        object Value
    );
}