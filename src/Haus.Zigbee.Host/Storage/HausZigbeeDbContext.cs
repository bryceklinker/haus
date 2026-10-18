using Haus.Zigbee.Host.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Zigbee.Host.Storage;

public class HausZigbeeDbContext(DbContextOptions<HausZigbeeDbContext> options) : DbContext(options)
{
    public DbSet<KnownZigbeeDeviceEntity> KnownDevices => Set<KnownZigbeeDeviceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KnownZigbeeDeviceEntity>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.ExternalId).IsUnique();
        });
        base.OnModelCreating(modelBuilder);
    }
}
