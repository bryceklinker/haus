using System;
using Humanizer;

namespace Haus.Site.Host.Shared.Dates;

public static class DateTimeExtensions
{
    public static string FormatTimestamp(this string timestamp)
    {
        return DateTimeOffset.TryParse(timestamp, out var date) ? date.FormatTimestamp() : timestamp;
    }

    public static string FormatTimestamp(this DateTimeOffset timestamp)
    {
        return timestamp.ToLocalTime().Humanize();
    }
}
