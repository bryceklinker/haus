using System;

namespace Haus.Site.Host.Shared.Formatting;

public static class DateTimeExtensions
{
    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
    
    public static string FormatTimestamp(this string timestamp)
    {
        return DateTimeOffset.TryParse(timestamp, out var date) ? date.FormatTimestamp() : timestamp;
    }

    public static string FormatTimestamp(this DateTimeOffset timestamp)
    {
        return timestamp.ToLocalTime().ToString(DateFormat);
    }

    public static string FormatTimestamp(this DateTime timestamp)
    {
        return timestamp.ToLocalTime().ToString(DateFormat);
    }
}
