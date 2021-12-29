using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Application;

[Collection(HausWebHostCollectionFixture.Name)]
public class ApplicationApiTests
{
    private readonly IHausApiClient _client;

    public ApplicationApiTests(HausWebHostApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task WhenGettingLatestVersionThenReturnsLatestReleaseOnGithub()
    {
        var latestVersion = await _client.GetLatestVersionAsync();

        latestVersion.Version.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task WhenGettingLatestPackagesThenReturnsLatestPackagesFromGitHub()
    {
        var result = await _client.GetLatestPackagesAsync();

        result.Count.Should().BeGreaterThan(0);
        result.Items.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task WhenDownloadingPackageThenReturnsDownloadablePackage()
    {
        var packagesResult = await _client.GetLatestPackagesAsync();

        var response = await _client.DownloadLatestPackageAsync(packagesResult.Items[0].Id);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.Content.Headers.ContentType.MediaType.Should().Be("application/octet-stream");
    }
}