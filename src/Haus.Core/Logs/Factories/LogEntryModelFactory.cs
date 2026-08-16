using System;
using System.Globalization;
using System.Text.Json;
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
        var value = JsonDocument.Parse(line).RootElement;

        var timestamp = GetValue(TimestampKey, value) ?? DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
        var level = GetValue(LevelKey, value) ?? LogLevel.Information.ToString();
        var message = GetValue(MessageKey, value) ?? "";
        return new LogEntryModel(timestamp, level, message, value);
    }

    private static string? GetValue(string key, JsonElement value)
    {
        return value.TryGetProperty(key, out var property) ? property.GetString() : null;
    }
}
