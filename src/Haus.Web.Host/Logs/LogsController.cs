using System.Threading.Tasks;
using Haus.Core.Logs.Queries;
using Haus.Core.Models.Logs;
using Haus.Cqrs;
using Haus.Hosting;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Logs;

[Route("api/logs")]
public class LogsController(IHausBus hausBus, ILogsDirectoryProvider logsDirectoryProvider)
    : HausBusController(hausBus)
{
    [HttpGet]
    public Task<IActionResult> GetLogs(
        [FromQuery] int pageSize = GetLogsParameters.DefaultPageSize,
        [FromQuery] int pageNumber = GetLogsParameters.DefaultPageNumber,
        [FromQuery] string searchTerm = null,
        [FromQuery] string level = null)
    {
        var query = new GetLogsQuery(
            logsDirectoryProvider.GetLogsDirectory(),
            new GetLogsParameters(pageNumber, pageSize, searchTerm, level)
        );
        return QueryAsync(query);
    }
}