using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Devices.Entities;

public class DeviceEndpointEntityConfiguration : IEntityTypeConfiguration<DeviceEndpointEntity>
{
    public void Configure(EntityTypeBuilder<DeviceEndpointEntity> builder)
    {
        builder.ToTable("DeviceEndpoints");
        builder.HasKey(d => d.Id);

        builder.Property(p => p.EndpointId).IsRequired();
        builder
            .Property(p => p.InClusters)
            .HasConversion(
                clusters => string.Join(",", clusters),
                value => ParseClusters(value),
                new ValueComparer<IReadOnlyList<ushort>>(
                    (left, right) => left != null && right != null && left.SequenceEqual(right),
                    clusters => clusters.Aggregate(0, (code, cluster) => HashCode.Combine(code, cluster.GetHashCode())),
                    clusters => clusters.ToList()
                )
            )
            .IsRequired();

        builder.HasOne(d => d.Device).WithMany(d => d.Endpoints);
    }

    private static IReadOnlyList<ushort> ParseClusters(string value)
    {
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(ushort.Parse).ToArray();
    }
}
