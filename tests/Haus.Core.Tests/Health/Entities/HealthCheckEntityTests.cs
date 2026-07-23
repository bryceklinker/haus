using System;
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

        Assert.Equal("What", entity.Name);
        Assert.Equal(HealthStatus.Degraded, entity.Status);
        Assert.Equal(66, entity.DurationOfCheckInMilliseconds);
        Assert.Equal("What up", entity.Description);
        Assert.Equal("not good", entity.ExceptionMessage);
        Assert.Contains("something", entity.Tags);
    }

    [Fact]
    public void WhenCreatedFromModelThenLastUpdatedIsSetToTimestamp()
    {
        var model = new HausHealthCheckModel("", HealthStatus.Healthy, 5);
        var timestamp = DateTimeOffset.UtcNow;

        var entity = HealthCheckEntity.FromModel(model, timestamp);

        Assert.Equal(timestamp, entity.LastUpdatedTimestamp);
    }

    [Fact]
    public void WhenUpdatedFromModelThenEntityIsPopulatedFromModel()
    {
        var model = new HausHealthCheckModel("three", HealthStatus.Unhealthy, 5, "good", "exception", ["boom"]);

        var entity = new HealthCheckEntity();
        entity.UpdateFromModel(model, DateTimeOffset.UtcNow);

        Assert.Equal("three", entity.Name);
        Assert.Equal(HealthStatus.Unhealthy, entity.Status);
        Assert.Equal(5, entity.DurationOfCheckInMilliseconds);
        Assert.Equal("good", entity.Description);
        Assert.Equal("exception", entity.ExceptionMessage);
        Assert.Contains("boom", entity.Tags);
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

        Assert.Equal("Checky", model.Name);
        Assert.Equal("Hola", model.Description);
        Assert.Equal(HealthStatus.Healthy, model.Status);
        Assert.Contains("welp", model.Tags);
        Assert.Equal("failure", model.ExceptionMessage);
        Assert.Equal(66, model.DurationOfCheckInMilliseconds);
    }

    [Fact]
    public void WhenConvertedToModelWithOldTimestampThenStatusIsUnhealthy()
    {
        var entity = new HealthCheckEntity
        {
            Status = HealthStatus.Healthy,
            LastUpdatedTimestamp = DateTimeOffset.UtcNow.AddHours(-1.1),
        };

        var model = entity.ToModel();

        Assert.Equal(HealthStatus.Unhealthy, model.Status);
    }
}
