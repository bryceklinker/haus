using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Application;
using Haus.Cqrs.Queries;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Application.Queries
{
    public record GetLatestVersionQuery : IQuery<ApplicationVersionModel>;

    internal class GetLatestVersionQueryHandler : IQueryHandler<GetLatestVersionQuery, ApplicationVersionModel>
    {
        private static readonly Version CurrentVersion = typeof(GetLatestVersionQuery).Assembly.GetName().Version;
        
        private readonly ILatestReleaseProvider _latestReleaseProvider;
        private readonly ILogger<GetLatestVersionQueryHandler> _logger;

        public GetLatestVersionQueryHandler(ILatestReleaseProvider latestReleaseProvider, ILogger<GetLatestVersionQueryHandler> logger)
        {
            _latestReleaseProvider = latestReleaseProvider;
            _logger = logger;
        }

        public async Task<ApplicationVersionModel> Handle(GetLatestVersionQuery request, CancellationToken cancellationToken)
        {
            var latestRelease = await TryGetLatestRelease();
            var isNewer = CurrentVersion < latestRelease.Version;
            return new ApplicationVersionModel(
                latestRelease.Version.ToSemanticVersion(), 
                latestRelease.IsOfficial, 
                isNewer,
                latestRelease.CreationDateTime,
                latestRelease.Description);
        }

        private async Task<ReleaseModel> TryGetLatestRelease()
        {
            try
            {
                return await _latestReleaseProvider.GetLatestVersionAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get the latest version using provider {Type}", _latestReleaseProvider.GetType());
                return new ReleaseModel(Version.Parse("0.0.0"), false, DateTimeOffset.MinValue, "");
            }
        }
    }
}