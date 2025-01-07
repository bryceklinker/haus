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
    Task<ListResult<LogEntryModel>> GetLogsAsync(GetLogsParameters parameters = null);
}

public class LogsApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options), ILogsApiClient
{
    public Task<ListResult<LogEntryModel>> GetLogsAsync(GetLogsParameters parameters = null)
    {
        return GetAsJsonAsync<ListResult<LogEntryModel>>("logs", ConvertToQueryParameters(parameters));
    }

    private static QueryParameters ConvertToQueryParameters(GetLogsParameters parameters)
    {
        if (parameters == null)
            return null;

        return new QueryParameters
        {
            { "pageSize", parameters.PageSize.ToString() },
            { "pageNumber", parameters.PageNumber.ToString() },
            { "search", parameters.SearchTerm },
            { "level", parameters.Level }
        };
    }
}