using Haus.Core.Lighting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Rooms.Entities;

public class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
{
    public void Configure(EntityTypeBuilder<RoomEntity> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired();
        builder.Property(r => r.OccupancyTimeoutInSeconds).IsRequired().HasDefaultValue(300);
        builder.Property(r => r.LastOccupiedTime);

        builder.OwnsOne(r => r.Lighting, LightingEntity.Configure);

        builder.Ignore(r => r.Lights);
        builder.HasMany(r => r.Devices)
            .WithOne(d => d.Room);
    }
}