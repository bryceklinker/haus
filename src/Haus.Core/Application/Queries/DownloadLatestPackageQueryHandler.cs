using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Queries;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Application.Queries;

public record DownloadLatestPackageQuery(int PackageId) : IQuery<DownloadLatestPackageResult>;

public class DownloadLatestPackageQueryHandler(
    ILatestReleaseProvider latestReleaseProvider,
    ILogger<DownloadLatestPackageQuery> logger)
    : IQueryHandler<DownloadLatestPackageQuery, DownloadLatestPackageResult>
{
    public async Task<DownloadLatestPackageResult> Handle(DownloadLatestPackageQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var stream = await latestReleaseProvider.DownloadLatestPackage(request.PackageId).ConfigureAwait(false);
            return DownloadLatestPackageResult.Ok(stream);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to download package from provider {Type}", latestReleaseProvider.GetType());
            return CreatePackageResultFromException(e);
        }
    }

    private DownloadLatestPackageResult CreatePackageResultFromException(Exception exception)
    {
        return exception switch
        {
            HttpRequestException httpException => CreatePackageResultFromHttpRequestException(httpException),
            _ => DownloadLatestPackageResult.Error()
        };
    }

    private DownloadLatestPackageResult CreatePackageResultFromHttpRequestException(HttpRequestException exception)
    {
        var status = exception.StatusCode.ToDownloadStatus();
        return DownloadLatestPackageResult.Error(status);
    }
}