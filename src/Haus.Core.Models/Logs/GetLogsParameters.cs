namespace Haus.Core.Models.Logs
{
    public record GetLogsParameters(
        int? PageNumber = GetLogsParameters.DefaultPageNumber,
        int? PageSize = GetLogsParameters.DefaultPageSize,
        string SearchTerm = null
    )
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 25;
        [OptionalGeneration] public string SearchTerm { get; } = SearchTerm;
    }
}