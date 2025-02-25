using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.Logs;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Logs;

[Collection(HausWebHostCollectionFixture.Name)]
public class LogsApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task WhenGettingLogsFromApiThenReturnsLogsFromLogFiles()
    {
        var logs = await _client.GetLogsAsync();

        logs.Count.Should().BeGreaterOrEqualTo(10);
        logs.Items.Length.Should().BeGreaterOrEqualTo(10);
    }

    [Fact]
    public async Task WhenGettingLogsFromApiUsingParametersThenReturnsLogsMeetingParameters()
    {
        var parameters = new GetLogsParameters(2, 5, "Haus", "Error");

        var logs = await _client.GetLogsAsync(parameters);

        logs.Items.Should().Match(entries => entries.All(e => e.Level == "Error"));
    }
}
