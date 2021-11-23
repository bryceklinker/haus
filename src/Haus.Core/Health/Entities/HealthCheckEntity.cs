using System;
using System.Linq.Expressions;
using Haus.Core.Common.Entities;
using Haus.Core.Models.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Core.Health.Entities
{
    public record HealthCheckEntity : Entity
    {
        private static DateTimeOffset OneHourAgo() => DateTimeOffset.UtcNow.AddHours(-1);
        public static readonly Expression<Func<HealthCheckEntity, HausHealthCheckModel>> ToModelExpression =
            c => new HausHealthCheckModel(
                c.Name, 
                c.LastUpdatedTimestamp >= OneHourAgo() ? c.Status : HealthStatus.Unhealthy, 
                c.DurationOfCheckInMilliseconds,
                c.Description,
                c.ExceptionMessage, 
                c.Tags
            );
        
        private static readonly Lazy<Func<HealthCheckEntity, HausHealthCheckModel>> ToModelFunc = new(ToModelExpression.Compile);
        
        public string Name { get; set; }
        public HealthStatus Status { get; set; }
        public string Description { get; set; }
        public string ExceptionMessage { get; set; }
        public double DurationOfCheckInMilliseconds { get; set; }
        public DateTimeOffset LastUpdatedTimestamp { get; set; }
        public string[] Tags { get; set; }

        public static HealthCheckEntity FromModel(HausHealthCheckModel model, DateTimeOffset timestamp)
        {
            var entity = new HealthCheckEntity();
            entity.UpdateFromModel(model, timestamp);
            return entity;
        }

        public void UpdateFromModel(HausHealthCheckModel model, DateTimeOffset timestamp)
        {
            Description = model.Description;
            DurationOfCheckInMilliseconds = model.DurationOfCheckInMilliseconds;
            ExceptionMessage = model.ExceptionMessage;
            Name = model.Name;
            Status = model.Status;
            Tags = model.Tags;
            LastUpdatedTimestamp = timestamp;
        }

        public HausHealthCheckModel ToModel()
        {
            return ToModelFunc.Value.Invoke(this);
        }
    }
}