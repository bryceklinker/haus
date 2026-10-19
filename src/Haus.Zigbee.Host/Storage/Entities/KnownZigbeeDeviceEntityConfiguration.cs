using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Zigbee.Host.Storage.Entities;

public class KnownZigbeeDeviceEntityConfiguration : IEntityTypeConfiguration<KnownZigbeeDeviceEntity>
{
    public void Configure(EntityTypeBuilder<KnownZigbeeDeviceEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ExternalId).IsUnique();
    }
}
