using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Haus.Zigbee.Host.Tests.Support;

// Each CreateLogger call gets its own logger instance tagged with its own category, all writing
// into one shared, lock-protected list -- so entries stay attributed to whichever type actually
// logged them, even though several types can share this one factory.
public class CapturingLoggerFactory : ILoggerFactory
{
    private readonly List<LogEntry> _entries = [];

    public IReadOnlyList<LogEntry> Entries
    {
        get
        {
            lock (_entries)
                return _entries.ToList();
        }
    }

    public ILogger CreateLogger(string categoryName) => new CategoryLogger(categoryName, _entries);

    public void AddProvider(ILoggerProvider provider) { }

    public void Dispose() { }

    private class CategoryLogger(string category, List<LogEntry> entries) : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            lock (entries)
                entries.Add(new LogEntry(category, logLevel, formatter(state, exception), exception));
        }
    }

    private class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose() { }
    }
}

public record LogEntry(string Category, LogLevel Level, string Message, Exception? Exception);
