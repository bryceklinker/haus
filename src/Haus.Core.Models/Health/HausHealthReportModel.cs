using System;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Core.Models.Health
{
    public record HausHealthReportModel(
        HealthStatus Status, 
        double DurationOfCheckInMilliseconds,
        HausHealthCheckModel[] Checks)
    {
        public bool IsOk { get; } = Status == HealthStatus.Healthy;
        public bool IsWarn { get; } = Status == HealthStatus.Degraded;
        public bool IsError => Status == HealthStatus.Unhealthy;

        public double DurationOfCheckInSeconds { get; } = TimeSpan.FromMilliseconds(DurationOfCheckInMilliseconds).TotalSeconds;

        public static HausHealthReportModel FromHealthReport(HealthReport report)
        {
            var checks = report.Entries.Select(HausHealthCheckModel.FromHealthReportEntry).ToArray();
            return new HausHealthReportModel(report.Status, report.TotalDuration.TotalMilliseconds, checks);
        }
    }
}