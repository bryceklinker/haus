using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Common;
using Haus.Api.Client.Options;
using Haus.Core.Models.Application;
using Haus.Core.Models.Common;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Application;

public interface IApplicationApiClient
{
    Task<ApplicationVersionModel?> GetLatestVersionAsync();
    Task<ListResult<ApplicationPackageModel>> GetLatestPackagesAsync();
    Task<HttpResponseMessage> DownloadLatestPackageAsync(int packageId);
}

public class ApplicationApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    : ApiClient(httpClient, options),
        IApplicationApiClient
{
    private const string LatestVersionRoute = "application/latest-version";

    public Task<ApplicationVersionModel?> GetLatestVersionAsync()
    {
        return GetAsJsonAsync<ApplicationVersionModel>(LatestVersionRoute);
    }

    public async Task<ListResult<ApplicationPackageModel>> GetLatestPackagesAsync()
    {
        return await GetAsJsonAsync<ListResult<ApplicationPackageModel>>($"{LatestVersionRoute}/packages")
            ?? new ListResult<ApplicationPackageModel>();
    }

    public Task<HttpResponseMessage> DownloadLatestPackageAsync(int packageId)
    {
        var fullUrl = GetFullUrl($"{LatestVersionRoute}/packages/{packageId}/download");
        return HttpClient.GetAsync(fullUrl);
    }
}
