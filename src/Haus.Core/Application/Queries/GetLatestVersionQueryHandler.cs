using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Application;
using Haus.Cqrs.Queries;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Application.Queries;

public record GetLatestVersionQuery : IQuery<ApplicationVersionModel>;

internal class GetLatestVersionQueryHandler(
    ILatestReleaseProvider latestReleaseProvider,
    ILogger<GetLatestVersionQueryHandler> logger
) : IQueryHandler<GetLatestVersionQuery, ApplicationVersionModel>
{
    public async Task<ApplicationVersionModel> Handle(
        GetLatestVersionQuery request,
        CancellationToken cancellationToken
    )
    {
        var latestRelease = await TryGetLatestRelease();
        var isNewer = GetCurrentVersion() < latestRelease.Version;
        return new ApplicationVersionModel(
            latestRelease.Version.ToSemanticVersion(),
            latestRelease.IsOfficial,
            isNewer,
            latestRelease.CreationDateTime,
            latestRelease.Description
        );
    }

    private async Task<ReleaseModel> TryGetLatestRelease()
    {
        try
        {
            var latest = await latestReleaseProvider.GetLatestVersionAsync();
            return latest ?? ReleaseModel.Default;
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Failed to get the latest version using provider {Type}",
                latestReleaseProvider.GetType()
            );
            return ReleaseModel.Default;
        }
    }

    private static Version GetCurrentVersion()
    {
        var version = typeof(GetLatestVersionQuery).Assembly.GetName().Version;
        return version == null ? Version.Parse("0.0.0") : version;
    }
}
