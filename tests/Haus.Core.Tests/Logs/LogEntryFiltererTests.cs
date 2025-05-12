using System.Dynamic;
using FluentAssertions;
using Haus.Core.Logs;
using Haus.Core.Models.Logs;
using Xunit;

namespace Haus.Core.Tests.Logs;

public class LogEntryFiltererTests
{
    private readonly LogEntryFilterer _filterer = new();

    [Fact]
    public void WhenEntriesAreFilteredWithNullParametersThenReturnsEntries()
    {
        var entries = new[]
        {
            new LogEntryModel("", "", "", new ExpandoObject()),
            new LogEntryModel("", "", "", new ExpandoObject()),
            new LogEntryModel("", "", "", new ExpandoObject()),
        };

        var filtered = _filterer.Filter(entries, null);

        filtered.Should().HaveCount(3);
    }

    [Fact]
    public void WhenEntriesAreFilteredBySearchTermThenReturnsEntriesWithSearchTermInMessage()
    {
        var entries = new[]
        {
            new LogEntryModel("", "", "hi sue", new ExpandoObject()),
            new LogEntryModel("", "", "nope", new ExpandoObject()),
            new LogEntryModel("", "", "hi bob", new ExpandoObject()),
        };

        var filtered = _filterer.Filter(entries, new GetLogsParameters(SearchTerm: "hi"));

        filtered.Should().HaveCount(2);
    }

    [Fact]
    public void WhenEntriesAreFilteredBySearchTermThenReturnsEntriesWithSearchTermInMessageIgnoringCase()
    {
        var entries = new[]
        {
            new LogEntryModel("", "", "HI sue", new ExpandoObject()),
            new LogEntryModel("", "", "nope", new ExpandoObject()),
            new LogEntryModel("", "", "hi bob", new ExpandoObject()),
        };

        var filtered = _filterer.Filter(entries, new GetLogsParameters(SearchTerm: "Hi"));

        filtered.Should().HaveCount(2);
    }

    [Fact]
    public void WhenEntriesAreFilteredByLevelThenReturnsEntriesWithMatchingLevel()
    {
        var entries = new[]
        {
            new LogEntryModel("", "Error", "", new ExpandoObject()),
            new LogEntryModel("", "Information", "", new ExpandoObject()),
            new LogEntryModel("", "Warning", "", new ExpandoObject()),
        };

        var filtered = _filterer.Filter(entries, new GetLogsParameters(Level: "Warning"));

        filtered.Should().HaveCount(1);
    }

    [Fact]
    public void WhenEntriesAreFilteredByLevelThenReturnsEntriesMatchingLevelIgnoringCase()
    {
        var entries = new[]
        {
            new LogEntryModel("", "information", "", new ExpandoObject()),
            new LogEntryModel("", "INFORMATION", "", new ExpandoObject()),
            new LogEntryModel("", "error", "", new ExpandoObject()),
        };

        var filtered = _filterer.Filter(entries, new GetLogsParameters(Level: "information"));

        filtered.Should().HaveCount(2);
    }
}
