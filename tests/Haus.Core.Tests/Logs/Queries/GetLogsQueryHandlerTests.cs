using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Logs.Queries;
using Haus.Core.Models.Logs;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Logs.Queries;

public class GetLogsQueryHandlerTests
{
    private readonly string _logsDirectory = Path.Combine(
        Directory.GetCurrentDirectory(),
        "..",
        "..",
        "..",
        "Logs",
        "sample-log-files"
    );
    private readonly IHausBus _hausBus = HausBusFactory.Create();

    [Fact]
    public async Task WhenGettingWithNoParametersThenFirst25LogsAreReturned()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory));

        Assert.Equal(25, result.Count);
        Assert.Equal(25, result.Items.Length);
    }

    [Fact]
    public async Task WhenGettingWithNoParametersThenLogsAreOrderedNewestToOldest()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory));

        Assert.Equal("2021-01-17T15:27:50.5660960Z", result.Items[0].Timestamp);
        Assert.Equal("2021-01-17T15:27:50.5659580Z", result.Items[1].Timestamp);
    }

    [Fact]
    public async Task WhenGettingSecondPageOfLogsThenSecondSetOfLogsIsReturned()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory, new GetLogsParameters(2)));

        Assert.Equal(25, result.Count);
        Assert.Equal("2021-01-17T15:27:20.6306500Z", result.Items[0].Timestamp);
        Assert.Equal("2021-01-17T15:27:20.6258920Z", result.Items[1].Timestamp);
    }

    [Fact]
    public async Task WhenQueryContainsSearchTermThenReturnsEntriesWithMessageContainingTerm()
    {
        var parameters = new GetLogsParameters(SearchTerm: "Entity Framework Core");
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory, parameters));

        Assert.Equal("2021-01-17T15:27:45.6356650Z", result.Items[0].Timestamp);
    }

    [Fact]
    public async Task WhenQueryContainsSearchTermThenMatchingTermIgnoresCase()
    {
        var parameters = new GetLogsParameters(SearchTerm: "entity FRAMEWORK core");
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory, parameters));

        Assert.Equal("2021-01-17T15:27:45.6356650Z", result.Items[0].Timestamp);
    }

    [Fact]
    public async Task WhenQueryContainsLevelThenAllReturnedLogsHaveSpecifiedLevel()
    {
        var parameters = new GetLogsParameters(Level: "Error");

        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory, parameters));

        Assert.True(result.Items.All(l => l.Level == "Error"));
    }
}
