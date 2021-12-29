using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Core.Models.Health;

public record HausHealthReportModel(
    HealthStatus Status,
    double DurationOfCheckInMilliseconds,
    HausHealthCheckModel[] Checks)
{
    public bool IsOk { get; } = Status == HealthStatus.Healthy;
    public bool IsWarn { get; } = Status == HealthStatus.Degraded;
    public bool IsError => Status == HealthStatus.Unhealthy;

    public HausHealthCheckModel[] Checks { get; } = Checks ?? Array.Empty<HausHealthCheckModel>();

    public double DurationOfCheckInSeconds { get; } =
        TimeSpan.FromMilliseconds(DurationOfCheckInMilliseconds).TotalSeconds;

    public static HausHealthReportModel FromHealthReport(HealthReport report)
    {
        var checks = report.Entries.Select(HausHealthCheckModel.FromHealthReportEntry).ToArray();
        return new HausHealthReportModel(report.Status, report.TotalDuration.TotalMilliseconds, checks);
    }

    public HausHealthReportModel AppendChecks(IEnumerable<HausHealthCheckModel> checksToAppend)
    {
        var checks = Checks.Concat(checksToAppend).ToArray();
        var totalDuration = checks.Sum(c => c.DurationOfCheckInMilliseconds);
        return new HausHealthReportModel(
            GetReportStatusFromChecks(checks),
            totalDuration,
            checks);
    }

    private HealthStatus GetReportStatusFromChecks(HausHealthCheckModel[] checks)
    {
        if (checks.Any(c => c.Status == HealthStatus.Unhealthy))
            return HealthStatus.Unhealthy;

        if (checks.Any(c => c.Status == HealthStatus.Degraded))
            return HealthStatus.Degraded;

        return Status;
    }
}