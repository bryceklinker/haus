using System;
using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models.Logs;

namespace Haus.Core.Logs;

public interface ILogEntryFilterer
{
    IEnumerable<LogEntryModel> Filter(IEnumerable<LogEntryModel> entries, GetLogsParameters parameters);
}

public class LogEntryFilterer : ILogEntryFilterer
{
    public IEnumerable<LogEntryModel> Filter(IEnumerable<LogEntryModel> entries, GetLogsParameters parameters)
    {
        if (parameters == null)
            return entries;

        return entries.Where(entry => DoesEntryMatchParameters(entry, parameters));
    }

    private static bool DoesEntryMatchParameters(LogEntryModel entry, GetLogsParameters query)
    {
        return DoesEntryMatchLevel(entry, query.Level)
               && DoesEntryContainSearchTerm(entry, query.SearchTerm);
    }

    private static bool DoesEntryContainSearchTerm(LogEntryModel entry, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return true;

        return entry.Message.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
    }

    private static bool DoesEntryMatchLevel(LogEntryModel entry, string level)
    {
        if (string.IsNullOrWhiteSpace(level))
            return true;

        return entry.Level.Equals(level, StringComparison.OrdinalIgnoreCase);
    }
}