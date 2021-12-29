using System;
using Microsoft.EntityFrameworkCore;
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
            tags => tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
        );
    }
}