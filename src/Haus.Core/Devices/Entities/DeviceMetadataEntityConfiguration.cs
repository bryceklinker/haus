using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Devices.Entities;

public class DeviceMetadataEntityConfiguration : IEntityTypeConfiguration<DeviceMetadataEntity>
{
    public void Configure(EntityTypeBuilder<DeviceMetadataEntity> builder)
    {
        builder.ToTable("DeviceMetadata");
        builder.HasKey(d => d.Id);

        builder.Property(p => p.Key).IsRequired();
        builder.Property(p => p.Value).IsRequired();
        builder.HasOne(d => d.Device)
            .WithMany(d => d.Metadata);
    }
}