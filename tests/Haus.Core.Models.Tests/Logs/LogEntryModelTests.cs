using System.Text.Json;
using Haus.Core.Models.Logs;
using Xunit;

namespace Haus.Core.Models.Tests.Logs;

public class LogEntryModelTests
{
    private const string Json = """
        {"timestamp":"2021-01-17T15:27:12.3639990Z","level":"Debug","message":"boom","value":{"@t":"2021-01-17T15:27:12.3639990Z","nested":{"count":413}}}
        """;

    [Fact]
    public void WhenLogEntryModelJsonIsDeserializedWithDefaultOptionsThenDoesNotThrow()
    {
        var entry = HausJsonSerializer.Deserialize<LogEntryModel>(Json);

        Assert.NotNull(entry);
    }

    [Fact]
    public void WhenLogEntryModelJsonIsDeserializedThenValueContainsOriginalProperties()
    {
        var entry = HausJsonSerializer.Deserialize<LogEntryModel>(Json)!;

        Assert.Equal(JsonValueKind.Object, entry.Value.ValueKind);
        Assert.Equal(413, entry.Value.GetProperty("nested").GetProperty("count").GetInt32());
    }
}
