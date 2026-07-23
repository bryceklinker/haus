using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Application;

[Collection(HausWebHostCollectionFixture.Name)]
public class ApplicationApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task WhenGettingLatestVersionThenReturnsLatestReleaseOnGithub()
    {
        var latestVersion = await _client.GetLatestVersionAsync();

        if (latestVersion != null)
        {
            Assert.False(string.IsNullOrWhiteSpace(latestVersion.Version));
        }
    }

    [Fact]
    public async Task WhenGettingLatestPackagesThenReturnsLatestPackagesFromGitHub()
    {
        var result = await _client.GetLatestPackagesAsync();

        Assert.True(result.Count > 0);
        Assert.True(result.Items.Length > 0);
    }

    [Fact]
    public async Task WhenDownloadingPackageThenReturnsDownloadablePackage()
    {
        var packagesResult = await _client.GetLatestPackagesAsync();

        var response = await _client.DownloadLatestPackageAsync(packagesResult.Items[0].Id);

        Assert.True(response.IsSuccessStatusCode);
        if (response.Content.Headers.ContentType != null)
        {
            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.MediaType);
        }
    }
}
