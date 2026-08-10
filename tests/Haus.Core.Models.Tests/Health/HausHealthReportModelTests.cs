using System;
using System.Linq;
using Haus.Core.Models.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Core.Models.Tests.Health;

public class HausHealthReportModelTests
{
    [Fact]
    public void WhenChecksAreAppendedThenReturnsReportWithAdditionalChecks()
    {
        var checksToAppend = new[] { new HausHealthCheckModel("one", HealthStatus.Healthy, 4) };

        var report = new HausHealthReportModel(
            HealthStatus.Healthy,
            4,
            [new HausHealthCheckModel("boom", HealthStatus.Healthy, 4)]
        ).AppendChecks(checksToAppend);

        Assert.Equal(2, report.Checks.Count());
    }

    [Fact]
    public void WhenChecksAreAppendedWithUnhealthyStatusThenReportStatusIsUnhealthy()
    {
        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, []).AppendChecks([
            new HausHealthCheckModel("one", HealthStatus.Unhealthy, 1),
        ]);

        Assert.Equal(HealthStatus.Unhealthy, report.Status);
    }

    [Fact]
    public void WhenChecksAreAppendedWithDegradedStatusThenReportIsDegraded()
    {
        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, []).AppendChecks([
            new HausHealthCheckModel("one", HealthStatus.Degraded, 1),
        ]);

        Assert.Equal(HealthStatus.Degraded, report.Status);
    }

    [Fact]
    public void WhenChecksAreAppendedThenSumsEachCheckAsDurationForReport()
    {
        var report = new HausHealthReportModel(
            HealthStatus.Healthy,
            5,
            new[] { new HausHealthCheckModel("", HealthStatus.Degraded, 5) }
        ).AppendChecks([new HausHealthCheckModel("", HealthStatus.Healthy, 4)]);

        Assert.Equal(9, report.DurationOfCheckInMilliseconds);
    }

    [Fact]
    public void WhenChecksAreEmptyThenDurationIsZero()
    {
        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, []).AppendChecks(
            Array.Empty<HausHealthCheckModel>()
        );

        Assert.Equal(0, report.DurationOfCheckInMilliseconds);
    }
}
