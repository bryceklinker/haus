using System.Dynamic;
using System.Linq;
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

        Assert.Equal(3, filtered.Count());
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

        Assert.Equal(2, filtered.Count());
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

        Assert.Equal(2, filtered.Count());
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

        Assert.Single(filtered);
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

        Assert.Equal(2, filtered.Count());
    }
}
