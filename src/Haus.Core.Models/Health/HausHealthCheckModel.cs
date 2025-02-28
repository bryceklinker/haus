using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Core.Models.Health;

public record HausHealthCheckModel(
    string Name,
    HealthStatus Status,
    double DurationOfCheckInMilliseconds,
    [property: OptionalGeneration] string? Description = null,
    string? ExceptionMessage = null,
    string[]? Tags = null
)
{
    public bool IsOk => Status == HealthStatus.Healthy;
    public bool IsWarn { get; } = Status == HealthStatus.Degraded;
    public bool IsError => Status == HealthStatus.Unhealthy;

    public double DurationOfCheckInSeconds => TimeSpan.FromMilliseconds(DurationOfCheckInMilliseconds).TotalSeconds;

    public string[] Tags { get; init; } = Tags ?? [];

    public static HausHealthCheckModel FromHealthReportEntry(KeyValuePair<string, HealthReportEntry> entry)
    {
        return FromHealthReportEntry(entry.Key, entry.Value);
    }

    public static HausHealthCheckModel FromHealthReportEntry(string key, HealthReportEntry entry)
    {
        return new HausHealthCheckModel(
            key,
            entry.Status,
            entry.Duration.TotalMilliseconds,
            entry.Description,
            entry.Exception?.Message,
            entry.Tags.ToArray()
        );
    }
}
