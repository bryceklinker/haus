using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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

        result.Count.Should().Be(25);
        result.Items.Should().HaveCount(25);
    }

    [Fact]
    public async Task WhenGettingWithNoParametersThenLogsAreOrderedNewestToOldest()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory));

        result.Items[0].Timestamp.Should().Be("2021-01-17T15:27:50.5660960Z");
        result.Items[1].Timestamp.Should().Be("2021-01-17T15:27:50.5659580Z");
    }

    [Fact]
    public async Task WhenGettingSecondPageOfLogsThenSecondSetOfLogsIsReturned()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory, new GetLogsParameters(2)));

        result.Count.Should().Be(25);
        result.Items[0].Timestamp.Should().Be("2021-01-17T15:27:20.6306500Z");
        result.Items[1].Timestamp.Should().Be("2021-01-17T15:27:20.6258920Z");
    }

    [Fact]
    public async Task WhenQueryContainsSearchTermThenReturnsEntriesWithMessageContainingTerm()
    {
        var parameters = new GetLogsParameters(SearchTerm: "Entity Framework Core");
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory, parameters));

        result.Items[0].Timestamp.Should().Be("2021-01-17T15:27:45.6356650Z");
    }

    [Fact]
    public async Task WhenQueryContainsSearchTermThenMatchingTermIgnoresCase()
    {
        var parameters = new GetLogsParameters(SearchTerm: "entity FRAMEWORK core");
        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory, parameters));

        result.Items[0].Timestamp.Should().Be("2021-01-17T15:27:45.6356650Z");
    }

    [Fact]
    public async Task WhenQueryContainsLevelThenAllReturnedLogsHaveSpecifiedLevel()
    {
        var parameters = new GetLogsParameters(Level: "Error");

        var result = await _hausBus.ExecuteQueryAsync(new GetLogsQuery(_logsDirectory, parameters));

        result.Items.Should().Match(logs => logs.All(l => l.Level == "Error"));
    }
}
