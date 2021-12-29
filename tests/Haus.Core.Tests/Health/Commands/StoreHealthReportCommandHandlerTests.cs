using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Health.Commands;
using Haus.Core.Health.Entities;
using Haus.Core.Models.Health;
using Haus.Cqrs;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Core.Tests.Health.Commands;

public class StoreHealthReportCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly FakeClock _clock;
    private readonly IHausBus _bus;

    public StoreHealthReportCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _clock = new FakeClock();
        _bus = HausBusFactory.Create(_context, services => services.Replace<IClock>(_clock));
    }

    [Fact]
    public async Task WhenHealthReportIsStoredThenReportCheckIsAddedToDatabase()
    {
        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, new[]
        {
            new HausHealthCheckModel("NewHotness", HealthStatus.Healthy, 6)
        });

        await _bus.ExecuteCommandAsync(new StoreHealthReportCommand(report));

        _context.GetAll<HealthCheckEntity>().Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenHealthReportHasExistingChecksThenChecksAreUpdated()
    {
        var now = new DateTimeOffset(2020, 3, 27, 2, 1, 5, TimeSpan.Zero);
        _clock.SetNow(now);

        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, new[]
        {
            new HausHealthCheckModel("NewHotness", HealthStatus.Healthy, 6)
        });
        _context.AddHealthCheck("NewHotness");

        await _bus.ExecuteCommandAsync(new StoreHealthReportCommand(report));

        _context.GetAll<HealthCheckEntity>().Should().HaveCount(1)
            .And.Contain(c => c.Name == "NewHotness")
            .And.Contain(c => c.LastUpdatedTimestamp == now);
    }

    [Fact]
    public async Task WhenHealthReportContainsMultipleChecksThenAllChecksAreAddedToDatabase()
    {
        var report = new HausHealthReportModel(HealthStatus.Healthy, 0, new[]
        {
            new HausHealthCheckModel("NewHotness", HealthStatus.Healthy, 6),
            new HausHealthCheckModel("Old n' Busted", HealthStatus.Healthy, 6)
        });

        await _bus.ExecuteCommandAsync(new StoreHealthReportCommand(report));

        _context.GetAll<HealthCheckEntity>().Should().HaveCount(2);
    }
}