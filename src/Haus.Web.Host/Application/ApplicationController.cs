using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Haus.Core;
using Haus.Core.Application.Queries;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Application;

[Route("api/application")]
public class ApplicationController : HausBusController
{
    public ApplicationController(IHausBus hausBus)
        : base(hausBus)
    {
    }

    [HttpGet("latest-version")]
    public Task<IActionResult> GetLatestVersion()
    {
        return QueryAsync(new GetLatestVersionQuery());
    }

    [HttpGet("latest-version/packages")]
    public Task<IActionResult> GetLatestPackages()
    {
        return QueryAsync(new GetLatestVersionPackagesQuery());
    }

    [HttpGet("latest-version/packages/{id}/download")]
    public async Task<IActionResult> DownloadPackage([FromRoute] int id)
    {
        var result = await Bus.ExecuteQueryAsync(new DownloadLatestPackageQuery(id)).ConfigureAwait(false);
        var httpStatusCode = result.Status.ToHttpStatus();
        return httpStatusCode switch
        {
            HttpStatusCode.OK => File(result.Bytes, MediaTypeNames.Application.Octet),
            _ => StatusCode((int)httpStatusCode)
        };
    }
}