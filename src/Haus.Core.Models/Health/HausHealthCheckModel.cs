using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Core.Models.Health
{
    public record HausHealthCheckModel(
        string Name,
        HealthStatus Status, 
        string Description,
        double DurationOfCheckInMilliseconds, 
        string ExceptionMessage, 
        string[] Tags)
    {
        public bool IsOk => Status != HealthStatus.Healthy;
        public bool IsDown => Status == HealthStatus.Unhealthy;

        public double DurationOfCheckInSeconds => TimeSpan.FromMilliseconds(DurationOfCheckInMilliseconds).TotalSeconds;

        public static HausHealthCheckModel FromHealthReportEntry(KeyValuePair<string, HealthReportEntry> entry)
        {
            return FromHealthReportEntry(entry.Key, entry.Value);
        }
        
        public static HausHealthCheckModel FromHealthReportEntry(string key, HealthReportEntry entry)
        {
            return new(
                key,
                entry.Status,
                entry.Description,
                entry.Duration.TotalMilliseconds,
                entry.Exception?.Message,
                entry.Tags.ToArray()
            );
        }
    }
}