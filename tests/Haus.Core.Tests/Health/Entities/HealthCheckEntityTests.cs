using System;
using FluentAssertions;
using Haus.Core.Health.Entities;
using Haus.Core.Models.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Core.Tests.Health.Entities;

public class HealthCheckEntityTests
{
    [Fact]
    public void WhenCreatedFromModelThenPopulatesEntityFromModel()
    {
        var model = new HausHealthCheckModel("What", HealthStatus.Degraded, 66, "What up", "not good", ["something"]);

        var entity = HealthCheckEntity.FromModel(model, DateTimeOffset.UtcNow);

        entity.Name.Should().Be("What");
        entity.Status.Should().Be(HealthStatus.Degraded);
        entity.DurationOfCheckInMilliseconds.Should().Be(66);
        entity.Description.Should().Be("What up");
        entity.ExceptionMessage.Should().Be("not good");
        entity.Tags.Should().Contain("something");
    }

    [Fact]
    public void WhenCreatedFromModelThenLastUpdatedIsSetToTimestamp()
    {
        var model = new HausHealthCheckModel("", HealthStatus.Healthy, 5);
        var timestamp = DateTimeOffset.UtcNow;

        var entity = HealthCheckEntity.FromModel(model, timestamp);

        entity.LastUpdatedTimestamp.Should().Be(timestamp);
    }

    [Fact]
    public void WhenUpdatedFromModelThenEntityIsPopulatedFromModel()
    {
        var model = new HausHealthCheckModel("three", HealthStatus.Unhealthy, 5, "good", "exception", ["boom"]);

        var entity = new HealthCheckEntity();
        entity.UpdateFromModel(model, DateTimeOffset.UtcNow);

        entity.Name.Should().Be("three");
        entity.Status.Should().Be(HealthStatus.Unhealthy);
        entity.DurationOfCheckInMilliseconds.Should().Be(5);
        entity.Description.Should().Be("good");
        entity.ExceptionMessage.Should().Be("exception");
        entity.Tags.Should().Contain("boom");
    }

    [Fact]
    public void WhenConvertedToModelThenEntityPopulatesModel()
    {
        var entity = new HealthCheckEntity
        {
            Id = 55,
            Name = "Checky",
            Description = "Hola",
            Status = HealthStatus.Healthy,
            Tags = ["welp"],
            ExceptionMessage = "failure",
            DurationOfCheckInMilliseconds = 66,
            LastUpdatedTimestamp = DateTimeOffset.UtcNow,
        };

        var model = entity.ToModel();

        model.Name.Should().Be("Checky");
        model.Description.Should().Be("Hola");
        model.Status.Should().Be(HealthStatus.Healthy);
        model.Tags.Should().Contain("welp");
        model.ExceptionMessage.Should().Be("failure");
        model.DurationOfCheckInMilliseconds.Should().Be(66);
    }

    [Fact]
    public void WhenConvertedToModelWithOldTimestampThenStatusIsUnhealthy()
    {
        var entity = new HealthCheckEntity
        {
            Status = HealthStatus.Healthy,
            LastUpdatedTimestamp = DateTimeOffset.UtcNow.AddHours(-1),
        };

        var model = entity.ToModel();

        model.Status.Should().Be(HealthStatus.Unhealthy);
    }
}
