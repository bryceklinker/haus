using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Haus.Core.Application;
using Haus.Web.Host.Common.GitHub;
using Microsoft.Extensions.Options;
using Octokit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace Haus.Web.Host.Application
{
    public class GithubLatestReleaseProvider : ILatestReleaseProvider
    {
        private const string ProductHeaderName = "haus-app";
        private readonly IOptions<GitHubSettings> _options;
        private readonly IGitHubClient _githubClient;

        private string GitHubUsername => _options.Value.Username;
        private string GithubPersonalAccessToken => _options.Value.PersonalAccessToken;
        private string GithubRepository => _options.Value.RepositoryName;
        private string GithubRepositoryOwner => _options.Value.RepositoryOwner;

        public GithubLatestReleaseProvider(IOptions<GitHubSettings> options)
        {
            _options = options;
            _githubClient = new GitHubClient(new ProductHeaderValue(ProductHeaderName))
            {
                Credentials = new Credentials(GitHubUsername, GithubPersonalAccessToken)
            };
        }

        public async Task<ReleaseModel> GetLatestVersionAsync()
        {
            var githubRelease = await GetLatestGitHubRelease().ConfigureAwait(false);
            var version = githubRelease.TagName.Replace("v", "", StringComparison.OrdinalIgnoreCase);
            return new ReleaseModel(Version.Parse(version), !githubRelease.Prerelease, githubRelease.CreatedAt, githubRelease.Body);
        }

        public async Task<ReleasePackageModel[]> GetLatestPackages()
        {
            var githubRelease = await GetLatestGitHubRelease().ConfigureAwait(false);
            return githubRelease.Assets
                .Select(a => new ReleasePackageModel(a.Id, a.Name))
                .ToArray();
        }

        public async Task<byte[]> DownloadLatestPackage(int id)
        {
            var githubRelease = await GetLatestGitHubRelease().ConfigureAwait(false);
            var asset = githubRelease.Assets.Single(p => p.Id == id);
            var response = await _githubClient.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), MediaTypeNames.Application.Octet);
            if (response.HttpResponse.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException($"Failed to get asset {id} from GitHub", null, response.HttpResponse.StatusCode);
            
            return response.Body;
        }

        private Task<Release> GetLatestGitHubRelease()
        {
            return _githubClient.Repository.Release.GetLatest(GithubRepositoryOwner, GithubRepository);
        }
    }
}