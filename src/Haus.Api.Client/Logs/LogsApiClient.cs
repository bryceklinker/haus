using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Common;
using Haus.Core.Models.Logs;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Logs;

public interface ILogsApiClient
{
    Task<ListResult<LogEntryModel>> GetLogsAsync(GetLogsParameters? parameters = null);
}

public class LogsApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options),
        ILogsApiClient
{
    public async Task<ListResult<LogEntryModel>> GetLogsAsync(GetLogsParameters? parameters = null)
    {
        return await GetAsJsonAsync<ListResult<LogEntryModel>>("logs", ConvertToQueryParameters(parameters))
            ?? new ListResult<LogEntryModel>();
    }

    private static QueryParameters? ConvertToQueryParameters(GetLogsParameters? parameters)
    {
        if (parameters == null)
            return null;

        var queryParameters = new QueryParameters();
        if (parameters.PageSize != null)
            queryParameters.Add("pageSize", $"{parameters.PageSize}");
        if (parameters.PageNumber != null)
            queryParameters.Add("pageNumber", $"{parameters.PageNumber}");
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            queryParameters.Add("searchTerm", parameters.SearchTerm);
        if (!string.IsNullOrWhiteSpace(parameters.Level))
            queryParameters.Add("level", parameters.Level);
        return queryParameters;
    }
}
