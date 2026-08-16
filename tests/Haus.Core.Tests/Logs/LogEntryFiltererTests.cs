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
            new LogEntryModel("", "", "", default),
            new LogEntryModel("", "", "", default),
            new LogEntryModel("", "", "", default),
        };

        var filtered = _filterer.Filter(entries, null);

        Assert.Equal(3, filtered.Count());
    }

    [Fact]
    public void WhenEntriesAreFilteredBySearchTermThenReturnsEntriesWithSearchTermInMessage()
    {
        var entries = new[]
        {
            new LogEntryModel("", "", "hi sue", default),
            new LogEntryModel("", "", "nope", default),
            new LogEntryModel("", "", "hi bob", default),
        };

        var filtered = _filterer.Filter(entries, new GetLogsParameters(SearchTerm: "hi"));

        Assert.Equal(2, filtered.Count());
    }

    [Fact]
    public void WhenEntriesAreFilteredBySearchTermThenReturnsEntriesWithSearchTermInMessageIgnoringCase()
    {
        var entries = new[]
        {
            new LogEntryModel("", "", "HI sue", default),
            new LogEntryModel("", "", "nope", default),
            new LogEntryModel("", "", "hi bob", default),
        };

        var filtered = _filterer.Filter(entries, new GetLogsParameters(SearchTerm: "Hi"));

        Assert.Equal(2, filtered.Count());
    }

    [Fact]
    public void WhenEntriesAreFilteredByLevelThenReturnsEntriesWithMatchingLevel()
    {
        var entries = new[]
        {
            new LogEntryModel("", "Error", "", default),
            new LogEntryModel("", "Information", "", default),
            new LogEntryModel("", "Warning", "", default),
        };

        var filtered = _filterer.Filter(entries, new GetLogsParameters(Level: "Warning"));

        Assert.Single(filtered);
    }

    [Fact]
    public void WhenEntriesAreFilteredByLevelThenReturnsEntriesMatchingLevelIgnoringCase()
    {
        var entries = new[]
        {
            new LogEntryModel("", "information", "", default),
            new LogEntryModel("", "INFORMATION", "", default),
            new LogEntryModel("", "error", "", default),
        };

        var filtered = _filterer.Filter(entries, new GetLogsParameters(Level: "information"));

        Assert.Equal(2, filtered.Count());
    }
}
