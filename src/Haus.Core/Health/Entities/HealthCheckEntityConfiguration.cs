using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Health.Entities;

public class HealthCheckEntityConfiguration : IEntityTypeConfiguration<HealthCheckEntity>
{
    public void Configure(EntityTypeBuilder<HealthCheckEntity> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Status).IsRequired();
        builder.Property(c => c.DurationOfCheckInMilliseconds).IsRequired();
        builder.Property(c => c.Tags).HasConversion(
            tags => string.Join(",", tags),
            tags => tags.Split(',', StringSplitOptions.RemoveEmptyEntries),
            new ValueComparer<string[]>(
                (left, right) => left != null 
                    ? left.SequenceEqual(right ?? Array.Empty<string>()) 
                    : right == null,
                c => c.Aggregate(0, (code, v) => HashCode.Combine(code, v.GetHashCode())),
                c => c.ToArray()
            )
        );
    }
}