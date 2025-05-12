using System;
using FluentAssertions;
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

        report.Checks.Should().HaveCount(2);
    }

    [Fact]
    public void WhenChecksAreAppendedWithUnhealthyStatusThenReportStatusIsUnhealthy()
    {
        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, []).AppendChecks(
            [new HausHealthCheckModel("one", HealthStatus.Unhealthy, 1)]
        );

        report.Status.Should().Be(HealthStatus.Unhealthy);
    }

    [Fact]
    public void WhenChecksAreAppendedWithDegradedStatusThenReportIsDegraded()
    {
        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, []).AppendChecks(
            [new HausHealthCheckModel("one", HealthStatus.Degraded, 1)]
        );

        report.Status.Should().Be(HealthStatus.Degraded);
    }

    [Fact]
    public void WhenChecksAreAppendedThenSumsEachCheckAsDurationForReport()
    {
        var report = new HausHealthReportModel(
            HealthStatus.Healthy,
            5,
            new[] { new HausHealthCheckModel("", HealthStatus.Degraded, 5) }
        ).AppendChecks([new HausHealthCheckModel("", HealthStatus.Healthy, 4)]);

        report.DurationOfCheckInMilliseconds.Should().Be(9);
    }

    [Fact]
    public void WhenChecksAreEmptyThenDurationIsZero()
    {
        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, []).AppendChecks(
            Array.Empty<HausHealthCheckModel>()
        );

        report.DurationOfCheckInMilliseconds.Should().Be(0);
    }
}
