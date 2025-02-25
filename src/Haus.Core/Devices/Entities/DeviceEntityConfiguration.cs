using Haus.Core.Lighting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Devices.Entities;

public class DeviceEntityConfiguration : IEntityTypeConfiguration<DeviceEntity>
{
    public void Configure(EntityTypeBuilder<DeviceEntity> builder)
    {
        builder.ToTable("Devices");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).IsRequired();
        builder.Property(d => d.ExternalId).IsRequired();
        builder.Property(d => d.DeviceType).IsRequired().HasConversion<string>();

        builder.Ignore(d => d.IsLight);

        builder.OwnsOne(d => d.Lighting, LightingEntity.Configure);

        builder.HasMany(d => d.Metadata).WithOne(m => m.Device);
    }
}
