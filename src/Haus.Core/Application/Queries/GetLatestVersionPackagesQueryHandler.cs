using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Application;
using Haus.Core.Models.Common;
using Haus.Cqrs.Queries;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Application.Queries;

public record GetLatestVersionPackagesQuery : IQuery<ListResult<ApplicationPackageModel>>;

internal class
    GetLatestVersionPackagesQueryHandler(
        ILatestReleaseProvider latestReleaseProvider,
        ILogger<GetLatestVersionPackagesQuery> logger)
    : IQueryHandler<GetLatestVersionPackagesQuery,
        ListResult<ApplicationPackageModel>>
{
    public async Task<ListResult<ApplicationPackageModel>> Handle(GetLatestVersionPackagesQuery request,
        CancellationToken cancellationToken)
    {
        var packages = await TryGetLatestPackages();
        return packages
            .Select(p => new ApplicationPackageModel(p.Id, p.Name))
            .OrderBy(p => p.Name)
            .ToListResult();
    }

    private async Task<ReleasePackageModel[]> TryGetLatestPackages()
    {
        try
        {
            return await latestReleaseProvider.GetLatestPackages();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get latest packages using provider {Type}",
                latestReleaseProvider.GetType());
            return [];
        }
    }
}