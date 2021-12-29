using System.Dynamic;
using System.Linq;
using System.Text.Json;
using Haus.Core.Logs.Queries;
using Haus.Core.Models;
using Haus.Core.Models.Logs;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Logs.Factories;

public interface ILogEntryModelFactory
{
    LogEntryModel CreateFromLine(string line);
}

public class LogEntryModelFactory : ILogEntryModelFactory
{
    private const string TimestampKey = "@t";
    private const string LevelKey = "@l";
    private const string MessageKey = "@m";

    public LogEntryModel CreateFromLine(string line)
    {
        ExpandoObject value = HausJsonSerializer.Deserialize<dynamic>(line,
            new JsonSerializerOptions(HausJsonSerializer.DefaultOptions)
            {
                Converters =
                {
                    new DynamicJsonConverter()
                }
            });

        var timestamp = GetValue(TimestampKey, value);
        var level = GetValue(LevelKey, value) ?? LogLevel.Information.ToString();
        var message = GetValue(MessageKey, value);
        return new LogEntryModel(timestamp, level, message, value);
    }

    private static string GetValue(string key, ExpandoObject value)
    {
        var result = value
            .Where(pair => pair.Key == key)
            .Select(pair => pair.Value)
            .FirstOrDefault();

        return result?.ToString();
    }
}